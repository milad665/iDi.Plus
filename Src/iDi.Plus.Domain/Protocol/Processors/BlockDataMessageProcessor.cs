using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;

namespace iDi.Plus.Domain.Protocol.Processors;

public class BlockDataMessageProcessor : MessageProcessorBase
{
    public BlockDataMessageProcessor(IBlockchainNodeClient blockchainNodeClient,
        IBlockchainRepository<IdTransaction> blockchainRepository, ILocalNodeContextProvider localNodeContextProvider,
        IBlockchainNodesProvider blockchainNodesProvider)
        : base(blockchainNodeClient, blockchainRepository, localNodeContextProvider, blockchainNodesProvider)
    {
    }

    public override MessageTypes MessageType => MessageTypes.BlockData;
    public override Message ProcessPayload(Message message)
    {
        throw new System.NotImplementedException();
    }
}