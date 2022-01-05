using iDi.Plus.Application.Context;
using iDi.Plus.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Exceptions;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Blockchain.Framework.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Application
{
    public class Process
    {
        private readonly Settings _settings;
        private readonly IBlockchainNodeServer _blockchainNodeServer;
        private readonly IBlockchainNodeClient _blockchainNodeClient;
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IdPlusDbContext _context;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public Process(Settings settings, IBlockchainNodeServer blockchainNodeServer, IdPlusDbContext context, 
            IBlockchainNodeClient blockchainNodeClient, IBlockchainRepository blockchainRepository)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _settings = settings;
            _blockchainNodeServer = blockchainNodeServer;
            _context = context;
            _blockchainNodeClient = blockchainNodeClient;
            _blockchainRepository = blockchainRepository;
        }

        public void Run()
        {
            var nodeKeys = LoadNodeKeys();
            var nodeId = nodeKeys.PublicKey.ToHexString();

            _context.ApplyMigrations(Seed);
            LoadDnsNodes();
            UpdateBlockchain(nodeId, nodeKeys.PrivateKey);

            _blockchainNodeServer.Listen(_settings.Port, _cancellationTokenSource.Token);
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
            Console.WriteLine("Node Keys Retrieved.");
            Console.WriteLine("Public Key:");
            Console.WriteLine(keys.PublicKey.ToHexString());
            Console.WriteLine();
            return keys;
        }

        private List<Node> LoadDnsNodes() => _context.Nodes.Where(n => n.IsDns && n.IpEndpoint != null).ToList();

        private void UpdateBlockchain(string nodeId, byte[] nodePrivateKey)
        {
            var node = _context.Nodes
                .OrderByDescending(n => n.LastHeartbeat)
                .FirstOrDefault(n => n.IsVerifierNode && n.IpEndpoint != null && n.LastHeartbeat != null);

            if (node == null)
                throw new NotFoundException("No verifier nodes found in the database.");

            var payload = GetNewBlocksPayload.Create(_blockchainRepository.GetLastBlockTimestamp());
            var header = Header.Create(Networks.Main, 1, node.PublicKey, MessageTypes.GetNewBlocks,
                payload.RawData.Length, payload.Sign(nodePrivateKey));
            var updateMessage = Message.Create(header, payload);
            var responseMessage = _blockchainNodeClient.Send(node.IpEndpoint, updateMessage);
            //use pipeline stage to process return data
            //var result = responseMessage.Process(nodeId, nodePrivateKey, _blockchainRepository);
            throw new NotImplementedException();
        }

        private void Seed(IdPlusDbContext context)
        {
            if (!context.Nodes.Any())
            {
                context.Nodes.Add(new Node(
                    "3059301306072A8648CE3D020106082A8648CE3D0301070342000417B99AED69CF040215D59769048CDC58E3C7B652EB5C4DFCD27CFEC6D2E3066F4A621902A7187838C1E25A2AABA79C370D4B4A804292B769B007BEDF04F18201",
                    true, true, new IPEndPoint(IPAddress.Loopback, FrameworkEnvironment.DefaultServerPort)));

                context.SaveChanges();
            }
        }
    }
}