using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;

namespace iDi.Plus.Domain.Protocol.Processors;

public class GetBlockMessageProcessor : MessageProcessorBase
{
    public GetBlockMessageProcessor(IBlockchainNodeClient blockchainNodeClient, IIdBlockchainRepository idBlockchainRepository, IHotPoolRepository<IdTransaction> hotPoolRepository, ILocalNodeContextProvider localNodeContextProvider, IBlockchainNodesRepository blockchainNodesRepository) : base(blockchainNodeClient, idBlockchainRepository, hotPoolRepository, localNodeContextProvider, blockchainNodesRepository)
    {
    }

    public override MessageTypes MessageType => MessageTypes.GetBlock;
    protected override Message ProcessPayload(Message message)
    {
        throw new System.NotImplementedException();
    }
}