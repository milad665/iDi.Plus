using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;

namespace iDi.Plus.Domain.Protocol.Processors;

public class GetTxMessageProcessor : MessageProcessorBase
{
    public GetTxMessageProcessor(IBlockchainNodeClient blockchainNodeClient,
        IBlockchainRepository<IdTransaction> blockchainRepository, IHotPoolRepository<IdTransaction> hotPoolRepository,
        ILocalNodeContextProvider localNodeContextProvider, IBlockchainNodesProvider blockchainNodesProvider) : base(
        blockchainNodeClient, blockchainRepository, hotPoolRepository, localNodeContextProvider,
        blockchainNodesProvider)
    {
    }

    public override MessageTypes MessageType => MessageTypes.GetTx;
    public override Message ProcessPayload(Message message)
    {
        throw new System.NotImplementedException();
    }
}