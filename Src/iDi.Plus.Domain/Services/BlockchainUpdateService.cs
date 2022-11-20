using System;
using System.Collections.Generic;
using System.Linq;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Exceptions;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Services;

public class BlockchainUpdateService : IBlockchainUpdateService
{
    private readonly IBlockchainUpdateServer _blockchainUpdateServer;
    private readonly IBlockchainNodeClient _blockchainNodeClient;
    private readonly ILocalNodeContextProvider _localNodeContextProvider;
    private readonly IBlockchainRepository<IdTransaction> _blockchainRepository;
    private readonly IBlockchainNodesRepository _blockchainNodesRepository;
    private readonly List<Block<IdTransaction>> _blocks;

    public BlockchainUpdateService(IBlockchainUpdateServer blockchainUpdateServer, IBlockchainNodeClient blockchainNodeClient, 
        ILocalNodeContextProvider localNodeContextProvider, IBlockchainRepository<IdTransaction> blockchainRepository, 
        IBlockchainNodesRepository blockchainNodesRepository)
    {
        _blockchainUpdateServer = blockchainUpdateServer;
        _blockchainNodeClient = blockchainNodeClient;
        _localNodeContextProvider = localNodeContextProvider;
        _blockchainRepository = blockchainRepository;
        _blockchainNodesRepository = blockchainNodesRepository;

        _blocks = new List<Block<IdTransaction>>();
    }

    public void Update(int serverPort)
    {
        _blockchainUpdateServer.ServerStarted += _blockchainUpdateServer_ServerStarted;
        _blockchainUpdateServer.WitnessNodesListMessageReceived += _blockchainUpdateServer_WitnessNodesListMessageReceived;
        _blockchainUpdateServer.NewBlocksMessageReceived += _blockchainUpdateServer_NewBlocksMessageReceived; ;
        _blockchainUpdateServer.BlockDataMessageReceived += _blockchainUpdateServer_BlockDataMessageReceived; ;
        _blockchainUpdateServer.AllBlocksReceived += _blockchainUpdateServer_AllBlocksReceived;
        _blockchainUpdateServer.Listen(serverPort);
    }

    private void _blockchainUpdateServer_ServerStarted()
    {
        var node = SelectAWitnessNode();

        var payload = GetWitnessNodesPayload.Create();
        var header = Header.Create(Networks.Main, 1, _localNodeContextProvider.LocalNodeId(), MessageTypes.GetWitnessNodes,
            payload.RawData.Length, payload.Sign(_localNodeContextProvider.LocalKeys.PrivateKey));
        var requestNodesMessage = Message.Create(header, payload);
        _blockchainNodeClient.Send(node.VerifiedEndpoint1,requestNodesMessage);
    }

    private void _blockchainUpdateServer_WitnessNodesListMessageReceived(IBlockchainUpdateServer arg1, MessageReceivedEventArgs arg2)
    {
        var message = arg2.Message;
        if (message.Payload is WitnessNodesList payload)
        {
            _blockchainNodesRepository.ReplaceAllNodes(payload.Nodes);
            
            if (!payload.Nodes.Any(n => n.NodeId.Equals(_localNodeContextProvider.LocalNodeId())))
            {
                _blockchainNodesRepository.AddOrUpdateNode(new BlockchainNode(_localNodeContextProvider.LocalNodeId(),
                    _localNodeContextProvider.IsWitnessNode, null, null, null, _localNodeContextProvider.IsDnsNode));
            }
        }

        RequestNewBlocks();
    }

    private void _blockchainUpdateServer_NewBlocksMessageReceived(IBlockchainUpdateServer arg1, MessageReceivedEventArgs arg2)
    {
        var message = arg2.Message;
        var payload = message.Payload as NewBlocksPayload;
        var cryptoServiceProvider = new CryptoServiceProvider();
        foreach (var blockHash in payload.Blocks)
        {
            var getBlockPayload = GetBlockPayload.Create(blockHash);
            var header = message.Header.ToResponseHeader(
                _localNodeContextProvider.LocalNodeId(), MessageTypes.GetBlock,
                getBlockPayload.RawData.Length,
                cryptoServiceProvider.Sign(_localNodeContextProvider.LocalKeys.PrivateKey, payload.RawData));
            var messageToSend = Message.Create(header, getBlockPayload);

            _blockchainNodeClient.Send(_blockchainNodesRepository[message.Header.NodeId].VerifiedEndpoint1, messageToSend);
        }
    }

    private void _blockchainUpdateServer_BlockDataMessageReceived(IBlockchainUpdateServer arg1, MessageReceivedEventArgs arg2)
    {
        var message = arg2.Message;
        var payload = message.Payload as BlockDataPayload;

        if (payload == null)
            throw new InvalidInputException("Invalid Block data payload");

        var transactions = new List<IdTransaction>();
        if (payload.Transactions == null)
            transactions = null;
        else
        {
            foreach (var tx in payload.Transactions)
            {
                if (tx.TransactionType == TransactionTypes.IssueTransaction)
                    transactions.Add(IssueIdTransaction.FromTxDataPayload(tx));
                else if (tx.TransactionType == TransactionTypes.ConsentTransaction)
                    transactions.Add(ConsentIdTransaction.FromTxDataPayload(tx));
                else
                    throw new NotSupportedException("Unsupported transaction type.");
            }
        }

        _blocks.Add(Block<IdTransaction>.Create(payload.Index, payload.PreviousHash, payload.Timestamp, transactions));
    }

    private void _blockchainUpdateServer_AllBlocksReceived()
    {
        AssertReceivedBlocksOrder();
        foreach (var block in _blocks.OrderBy(b => b.Index))
            _blockchainRepository.AddBlock(block);
    }


    private void RequestNewBlocks()
    {
        var node = SelectAWitnessNode();

        var payload = GetNewBlocksPayload.Create(_blockchainRepository.GetLastBlockIndex());
        var header = Header.Create(Networks.Main, 1, _localNodeContextProvider.LocalNodeId(), MessageTypes.GetNewBlocks,
            payload.RawData.Length, payload.Sign(_localNodeContextProvider.LocalKeys.PrivateKey));
        var updateMessage = Message.Create(header, payload);

        _blockchainNodeClient.Send(node.VerifiedEndpoint1, updateMessage);
    }
    private void AssertReceivedBlocksOrder()
    {
        var orderedBlocks = _blocks.OrderBy(b => b.Index).ToList();

        var minReceivedBlock = orderedBlocks.Last();
        
        if (minReceivedBlock.Index != _blockchainRepository.GetLastBlockIndex() + 1)
            throw new InvalidInputException("Received blocks do not follow the last block in the blockchain.");

        var lastBlockchainBlock = _blockchainRepository.GetLastBlock();
        if (lastBlockchainBlock == null && !orderedBlocks.First().IsGenesis())
            throw new InvalidInputException("Received blocks do not start with genesis block while no blockchain is stored.");


        var index = minReceivedBlock.Index + 1;
        var previousBlock = lastBlockchainBlock;
        foreach (var block in orderedBlocks)
        {
            if (index != block.Index)
                throw new VerificationFailedException("At least one block is missing.");
            if (previousBlock != null && !block.PreviousHash.Equals(previousBlock.Hash))
                throw new VerificationFailedException("Previous hash does not match the hash of the previous block.");
            
            previousBlock = block;

            index++;
        }
    }

    private BlockchainNode SelectAWitnessNode()
    {
        var node = _blockchainNodesRepository.AllNodes()
            .OrderByDescending(n => n.LastHeartbeatUtcTime)
            .FirstOrDefault(n => n.IsWitnessNode && n.VerifiedEndpoint1 != null && n.LastHeartbeatUtcTime != null);

        if (node == null)
            node = _blockchainNodesRepository.AllNodes()
                .OrderByDescending(n => n.LastHeartbeatUtcTime)
                .FirstOrDefault(n => n.IsWitnessNode && n.VerifiedEndpoint1 != null);

        if (node == null)
            throw new NotFoundException("Cannot find any witness node in the database.");

        return node;
    }
}