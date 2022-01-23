using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using Moq;

namespace iDi.Plus.Domain.Tests.Protocol.Processors;

public abstract class MessageProcessorTestBase
{

    protected MessageProcessorTestBase()
    {
        BlockchainNodeClientMock = new Mock<IBlockchainNodeClient>();
        BlockchainRepositoryMock = new Mock<IBlockchainRepository<IdTransaction>>();
        LocalNodeContextProviderMock = new Mock<ILocalNodeContextProvider>();
        BlockchainNodesProviderMock = new Mock<BlockchainNodesProvider>();

        SampleDataProvider = new SampleDataProvider();

        Setup();
    }

    private void Setup()
    {
        LocalNodeContextProviderMock.Setup(l => l.LocalKeys).Returns(SampleDataProvider.SampleLocalNodeKeys);
    }
    protected SampleDataProvider SampleDataProvider { get; }
    protected Mock<IBlockchainNodeClient> BlockchainNodeClientMock { get; }
    protected Mock<IBlockchainRepository<IdTransaction>> BlockchainRepositoryMock { get; }
    protected Mock<ILocalNodeContextProvider> LocalNodeContextProviderMock { get; }
    protected Mock<BlockchainNodesProvider> BlockchainNodesProviderMock { get; }
}