﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using Moq;

namespace iDi.Plus.Domain.Tests.Protocol.Processors;

public abstract class MessageProcessorTestBase
{
    protected Dictionary<string, BlockchainNode> RemoteNodes;

    protected MessageProcessorTestBase()
    {
        BlockchainNodeClientMock = new Mock<IBlockchainNodeClient>();
        BlockchainRepositoryMock = new Mock<IBlockchainRepository<IdTransaction>>();
        LocalNodeContextProviderMock = new Mock<ILocalNodeContextProvider>();
        BlockchainNodesProviderMock = new Mock<IBlockchainNodesProvider>();

        SampleDataProvider = new SampleDataProvider();

        Setup();
    }

    private void Setup()
    {
        LocalNodeContextProviderMock.Setup(l => l.LocalKeys).Returns(SampleDataProvider.SampleLocalNodeKeys);

        var nodeId1 = SampleDataProvider.SampleRemoteNodeKeys1.PublicKey.ToHexString();
        var nodeId2 = SampleDataProvider.SampleRemoteNodeKeys2.PublicKey.ToHexString();

        RemoteNodes = new Dictionary<string, BlockchainNode>
        {
            {nodeId1, new(nodeId1, true, new IPEndPoint(IPAddress.Loopback, 11000), new IPEndPoint(IPAddress.Loopback, 11001), DateTime.Now, true)},
            {nodeId2, new(nodeId2, true, new IPEndPoint(IPAddress.Loopback, 12000), new IPEndPoint(IPAddress.Loopback, 12001), DateTime.Now, true)}
        };
        BlockchainNodesProviderMock.Setup(p => p.AllNodeIds()).Returns(RemoteNodes.Keys);
        BlockchainNodesProviderMock.Setup(p => p.AllNodes()).Returns(RemoteNodes.Values);
        BlockchainNodesProviderMock.Setup(p => p.ToDictionary()).Returns(new ReadOnlyDictionary<string, BlockchainNode>(RemoteNodes));
        BlockchainNodesProviderMock.Setup(p => p[It.IsAny<string>()]).Returns((string nodeId) => RemoteNodes[nodeId]);
    }
    protected SampleDataProvider SampleDataProvider { get; }
    protected Mock<IBlockchainNodeClient> BlockchainNodeClientMock { get; }
    protected Mock<IBlockchainRepository<IdTransaction>> BlockchainRepositoryMock { get; }
    protected Mock<ILocalNodeContextProvider> LocalNodeContextProviderMock { get; }
    protected Mock<IBlockchainNodesProvider> BlockchainNodesProviderMock { get; }
}