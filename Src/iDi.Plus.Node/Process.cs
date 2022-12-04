using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using iDi.Plus.Domain.Services;
using iDi.Plus.Node.Context;
using Timer = System.Timers.Timer;

namespace iDi.Plus.Node;

public class Process
{
    private readonly Settings _settings;
    private readonly IBlockchainNodeServer _blockchainNodeServer;
    private readonly ILocalNodeContextProvider _localNodeContextProvider;
    private readonly IBlockchainUpdateService _blockchainUpdateService;
    private readonly IConsensusService _consensusService;
    private readonly IBlockchainNodesRepository _blockchainNodesRepository;
    private readonly IBlockchainNodeClient _blockchainNodeClient;

    private readonly IdPlusDbContext _context;
    private readonly CancellationTokenSource _cancellationTokenSource;

    private Timer _timer;
    private DateTime _lastWitnessNodeUpdateTime = DateTime.MinValue;
    
    public Process(Settings settings, 
        IBlockchainNodeServer blockchainNodeServer, 
        IdPlusDbContext context, 
        ILocalNodeContextProvider localNodeContextProvider, 
        IBlockchainUpdateService blockchainUpdateService, 
        IConsensusService consensusService, 
        IBlockchainNodesRepository blockchainNodesRepository, 
        IBlockchainNodeClient blockchainNodeClient)
    {
        _cancellationTokenSource = new CancellationTokenSource();

        _settings = settings;
        _blockchainNodeServer = blockchainNodeServer;
        _context = context;
        _localNodeContextProvider = localNodeContextProvider;
        _blockchainUpdateService = blockchainUpdateService;
        _consensusService = consensusService;
        _blockchainNodesRepository = blockchainNodesRepository;
        _blockchainNodeClient = blockchainNodeClient;
        
        _consensusService.BlockCreated += _consensusService_BlockCreated;

        InitializeTimer();
    }
    
    public void Run()
    {
        _localNodeContextProvider.LocalKeys = LoadNodeKeys();
        if (_settings.IsWitness)
            _localNodeContextProvider.SetWitnessNode();

        if (_settings.IsDns)
            _localNodeContextProvider.SetDnsNode();

        _context.ApplyMigrations(Seed);
        _blockchainUpdateService.Update(_settings.Port);
        _lastWitnessNodeUpdateTime = DateTime.Now;
        
        //Start the timer for time based recurring checks
        _timer.Enabled = true;

        _blockchainNodeServer.Listen(_settings.Port, _cancellationTokenSource.Token);
    }
    private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        if (_localNodeContextProvider.IsWitnessNode)
            _consensusService.ExecuteBlockCreationCycle();
        
        RequestWitnessNodeUpdateIfOutdated();
    }
    private void _consensusService_BlockCreated(ConsensusService arg1, BlockCreatedEventArgs arg2)
    {
        var witnessNodes = _blockchainNodesRepository.GetWitnessNodes();
        var bystanderNodes = _blockchainNodesRepository.GetBystanderNodes();

        var block = arg2.Block;
        var transactions = new List<TxDataResponsePayload>();
        foreach (var transaction in block.Transactions)
        {
            var verifier = AddressValue.Empty;
            if (transaction is ConsentIdTransaction consentIdTransaction)
                verifier = consentIdTransaction.VerifierAddress;

            var tx = TxDataResponsePayload.Create(transaction.TransactionHash, transaction.TransactionType,
                transaction.IssuerAddress, transaction.HolderAddress, verifier, transaction.Subject,
                transaction.IdentifierKey, transaction.Timestamp, transaction.PreviousTransactionHash,
                transaction.ValueMimeType, transaction.DoubleEncryptedData);
            transactions.Add(tx);
        }

        var payload = BlockDataPayload.Create(block.Index, block.Hash, block.PreviousHash, block.Timestamp,
            transactions, block.Nonce);

        var signedData = payload.Sign(_localNodeContextProvider.LocalKeys.PrivateKey);
        var header = Header.Create(Networks.Main, 1, _localNodeContextProvider.LocalNodeId(), MessageTypes.BlockData,
            payload.RawData.Length, signedData);
        var message = Message.Create(header, payload);

        foreach (var node in witnessNodes.Union(bystanderNodes))
            _blockchainNodeClient.Send(node.VerifiedEndpoint1, message);
    }
    private KeyPair LoadNodeKeys()
    {
        var path = Directory.GetCurrentDirectory();
        var files = Directory.GetFiles(path, "*.p12");
        if (files.Length == 0)
            Console.WriteLine("No p12 files found.");
        var file = "";
        if (files.Length == 1)
            file = files.First();
        else
        {
            Console.WriteLine($"Please choose a p12 file:");
            for (var i=1; i<=files.Length; i++)
                Console.WriteLine($"{i}.\t{Path.GetFileName(file)}");
            Console.Write($"Options: ");
            var option = Console.ReadLine();
            int.TryParse(option, out int selectedOption);
            if (selectedOption > 0 && selectedOption <= files.Length)
                file = files[selectedOption - 1];
        }

        Console.WriteLine("Selected file:");
        Console.WriteLine(file);
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        Console.Clear();
        Console.WriteLine("Enter Level-1 Password:");
        var l1Password = Console.ReadLine();
        Console.Clear();
        Console.WriteLine("Enter Level-2 Password:");
        var l2Password = Console.ReadLine();
        Console.Clear();

        using var fileStream = File.OpenRead(file);
        var buffer = new byte[fileStream.Length];
        fileStream.Read(buffer);

        var keys = DigitalSignatureKeys.FromPkcs12(buffer, l1Password, l2Password);
        Console.WriteLine("BlockchainNode Keys Retrieved.");
        Console.WriteLine("Public Key:");
        Console.WriteLine(keys.PublicKey.ToHexString());
        Console.WriteLine();
        return keys;
    }
    private void InitializeTimer()
    {
        _timer = new Timer(2000);
        _timer.Enabled = false;
        _timer.AutoReset = true;
        _timer.Elapsed += _timer_Elapsed;
    }

    private void RequestWitnessNodeUpdateIfOutdated()
    {
        if (DateTime.Now - _lastWitnessNodeUpdateTime < TimeSpan.FromMinutes(10)) 
            return;
        
        var node = _blockchainNodesRepository.GetOneRandomWitnessNode();

        var payload = GetWitnessNodesPayload.Create();
        var header = Header.Create(Networks.Main, 1, _localNodeContextProvider.LocalNodeId(), MessageTypes.GetWitnessNodes,
            payload.RawData.Length, payload.Sign(_localNodeContextProvider.LocalKeys.PrivateKey));
        var requestWitnessNodesListMessage = Message.Create(header, payload);
        _blockchainNodeClient.Send(node.VerifiedEndpoint1,requestWitnessNodesListMessage);
        _lastWitnessNodeUpdateTime = DateTime.Now;
    }
    private void Seed(IdPlusDbContext context)
    {
        if (!context.Nodes.Any())
        {
            context.Nodes.Add(new Domain.Entities.Node(
                new NodeIdValue("3059301306072A8648CE3D020106082A8648CE3D0301070342000417B99AED69CF040215D59769048CDC58E3C7B652EB5C4DFCD27CFEC6D2E3066F4A621902A7187838C1E25A2AABA79C370D4B4A804292B769B007BEDF04F18201"),
                true, new IPEndPoint(IPAddress.Loopback, FrameworkEnvironment.DefaultServerPort), null, null,
                true));

            context.SaveChanges();
        }
    }
}