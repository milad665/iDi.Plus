﻿using System.Linq;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Exceptions;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using iDi.Plus.Domain.Services;

namespace iDi.Plus.Domain.Protocol.Processors;

public class BlockDataMessageProcessor : MessageProcessorBase
{
    private readonly IBlockchain<IdTransaction> _blockchain;
    private readonly IIdTransactionFactory _idTransactionFactory;
    private readonly IConsensusService _consensusService;

    public BlockDataMessageProcessor(IBlockchainNodeClient blockchainNodeClient,
        IIdBlockchainRepository idBlockchainRepository,
        IHotPoolRepository<IdTransaction> hotPoolRepository,
        ILocalNodeContextProvider localNodeContextProvider,
        IBlockchainNodesRepository blockchainNodesRepository, 
        IBlockchain<IdTransaction> blockchain, 
        IIdTransactionFactory idTransactionFactory, 
        IConsensusService consensusService) : base(blockchainNodeClient, idBlockchainRepository,
        hotPoolRepository, localNodeContextProvider, blockchainNodesRepository)
    {
        _blockchain = blockchain;
        _idTransactionFactory = idTransactionFactory;
        _consensusService = consensusService;
    }

    public override MessageTypes MessageType => MessageTypes.BlockData;
    protected override Message ProcessPayload(Message message)
    {
        if (message.Payload is not BlockDataPayload payload)
            throw new InvalidInputException("Payload can not be cast to the target type of this processor.");

        var currentTurn = BlockchainNodesRepository.CurrentWitnessTurn();

        if (!BlockchainNodesRepository.GetWitnessNodes().Any(n => n.NodeId.Equals(message.Header.NodeId)))
            throw new VerificationFailedException("Sender is not a witness node.");

        if (currentTurn != null && !currentTurn.NodeId.Equals(message.Header.NodeId))
            throw new VerificationFailedException("Block sender node does not match the current turn.");

        var block = payload.ToBlock(_idTransactionFactory);
        
        if (LocalNodeContextProvider.IsWitnessNode)
        {
            VerifyBlockTransactions(block);
            HotPoolRepository.RemoveTransactions(block.Transactions);
        }
        
        block.VerifyHash();
        _blockchain.AddReceivedBlock(block);

        if (LocalNodeContextProvider.IsWitnessNode)
        {
            BlockchainNodesRepository.ClearVotes();
            _consensusService.VoteForNextNode();
            
            //send block to all non-witness nodes
            var bystanderNodes = BlockchainNodesRepository.GetBystanderNodes();
            foreach (var node in bystanderNodes)
                BlockchainNodeClient.Send(node.VerifiedEndpoint1, message);
        }

        return null;
    }

    private void VerifyBlockTransactions(Block<IdTransaction> block)
    {
        foreach (var tx in block.Transactions)
        {
            var hotPoolTx = HotPoolRepository.GetTransaction(tx.TransactionHash);
            if (hotPoolTx == null)
                throw new VerificationFailedException("Transaction not found in the hot pool");
            
            if (!hotPoolTx.Equals(tx))
                throw new VerificationFailedException("Block tx data does not match hot pool tx");
        }
    }
}