using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;

namespace iDi.Plus.Domain.Protocol.Processors;

public class NewTxsMessageProcessor : MessageProcessorBase
{
    public NewTxsMessageProcessor(IBlockchainNodeClient blockchainNodeClient, IBlockchainRepository<IdTransaction> blockchainRepository, ILocalNodeContextProvider localNodeContextProvider, IBlockchainNodesProvider blockchainNodesProvider) 
        : base(blockchainNodeClient, blockchainRepository, localNodeContextProvider, blockchainNodesProvider)
    {
    }

    public override MessageTypes MessageType => MessageTypes.NewTxs;
    public override Message ProcessPayload(Message message)
    {
        throw new System.NotImplementedException();
    }
}