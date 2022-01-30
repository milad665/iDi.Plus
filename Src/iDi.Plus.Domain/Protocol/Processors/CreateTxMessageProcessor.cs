using System;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Protocol.Processors;

public class CreateTxMessageProcessor : MessageProcessorBase
{
    public CreateTxMessageProcessor(IBlockchainNodeClient blockchainNodeClient, IBlockchainRepository<IdTransaction> blockchainRepository, ILocalNodeContextProvider localNodeContextProvider, IBlockchainNodesProvider blockchainNodesProvider) 
        : base(blockchainNodeClient, blockchainRepository, localNodeContextProvider, blockchainNodesProvider)
    {
    }

    public override MessageTypes MessageType => MessageTypes.CreateTx;
    public override Message ProcessPayload(Message message)
    {
        if (message.Payload is not CreateTxPayload payload)
            throw new InvalidInputException("Payload can not be cast to the target type of this processor.");

        throw new NotImplementedException();
    }
}