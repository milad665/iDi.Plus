﻿using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;

namespace iDi.Plus.Domain.Protocol.Processors;

public class VoteMessageProcessor : MessageProcessorBase
{
    public VoteMessageProcessor(IBlockchainNodeClient blockchainNodeClient, IBlockchainRepository<IdTransaction> blockchainRepository, IHotPoolRepository<IdTransaction> hotPoolRepository, ILocalNodeContextProvider localNodeContextProvider, IBlockchainNodesRepository blockchainNodesRepository) : base(blockchainNodeClient, blockchainRepository, hotPoolRepository, localNodeContextProvider, blockchainNodesRepository)
    {
    }

    public override MessageTypes MessageType => MessageTypes.Vote;
    public override Message ProcessPayload(Message message)
    {
        throw new System.NotImplementedException();
    }
}