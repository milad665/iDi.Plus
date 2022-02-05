using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using Moq;

namespace iDi.Plus.Domain.Tests.Protocol.Processors;

public abstract class MessageProcessorTestBase
{
    protected Dictionary<NodeIdValue, BlockchainNode> RemoteNodes;

    protected MessageProcessorTestBase()
    {
        BlockchainNodeClientMock = new Mock<IBlockchainNodeClient>();
        BlockchainRepositoryMock = new Mock<IBlockchainRepository<IdTransaction>>();
        HotPoolRepositoryMock = new Mock<IHotPoolRepository<IdTransaction>>();
        LocalNodeContextProviderMock = new Mock<ILocalNodeContextProvider>();
        BlockchainNodesProviderMock = new Mock<IBlockchainNodesProvider>();

        SampleDataProvider = new SampleDataProvider();

        Setup();
    }

    private void Setup()
    {
        LocalNodeContextProviderMock.Setup(l => l.LocalKeys).Returns(SampleDataProvider.SampleLocalNodeKeys);

        var nodeId1 = new NodeIdValue(SampleDataProvider.SampleRemoteNodeKeys1.PublicKey);
        var nodeId2 = new NodeIdValue(SampleDataProvider.SampleRemoteNodeKeys2.PublicKey);

        RemoteNodes = new Dictionary<NodeIdValue, BlockchainNode>
        {
            {nodeId1, new(nodeId1, true, new IPEndPoint(IPAddress.Loopback, 11000), new IPEndPoint(IPAddress.Loopback, 11001), DateTime.Now, true)},
            {nodeId2, new(nodeId2, true, new IPEndPoint(IPAddress.Loopback, 12000), new IPEndPoint(IPAddress.Loopback, 12001), DateTime.Now, true)}
        };

        BlockchainNodesProviderMock.Setup(p => p.AllNodeIds()).Returns(RemoteNodes.Keys);
        BlockchainNodesProviderMock.Setup(p => p.AllNodes()).Returns(RemoteNodes.Values);
        BlockchainNodesProviderMock.Setup(p => p.ToDictionary()).Returns(new ReadOnlyDictionary<NodeIdValue, BlockchainNode>(RemoteNodes));
        BlockchainNodesProviderMock.Setup(p => p[It.IsAny<NodeIdValue>()]).Returns((NodeIdValue nodeId) => RemoteNodes[nodeId]);
    }
    protected SampleDataProvider SampleDataProvider { get; }
    protected Mock<IBlockchainNodeClient> BlockchainNodeClientMock { get; }
    protected Mock<IBlockchainRepository<IdTransaction>> BlockchainRepositoryMock { get; }
    protected Mock<IHotPoolRepository<IdTransaction>> HotPoolRepositoryMock { get; }
    protected Mock<ILocalNodeContextProvider> LocalNodeContextProviderMock { get; }
    protected Mock<IBlockchainNodesProvider> BlockchainNodesProviderMock { get; }
}