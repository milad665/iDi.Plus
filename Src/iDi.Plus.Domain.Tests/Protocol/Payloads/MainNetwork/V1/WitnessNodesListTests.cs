using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol.Payloads.MainNetwork.V1;

public class WitnessNodesListTests: ProtocolsTestBase
{
    [Fact]
    public void RawDataCreatedSuccessfully()
    {
        var payload = WitnessNodesListPayload.Create(Nodes);
        var rawData = payload.RawData;
        var target = new WitnessNodesListPayload(rawData);
        var targetNodes = target.Nodes.ToList();
        for (var i = 0; i < Nodes.Count; i++)
        {
            Assert.Equal(Nodes[i].IsDns, targetNodes[i].IsDns);
            Assert.Equal(Nodes[i].NodeId, targetNodes[i].NodeId);
            Assert.Equal(Nodes[i].VerifiedEndpoint1, targetNodes[i].VerifiedEndpoint1);
            Assert.Equal(Nodes[i].VerifiedEndpoint2, targetNodes[i].VerifiedEndpoint2);
            Assert.Equal(Nodes[i].IsWitnessNode, targetNodes[i].IsWitnessNode);
            Assert.Null(targetNodes[i].LastHeartbeatUtcTime);
        }
    }

    private List<BlockchainNode> Nodes => new()
    {
        new BlockchainNode(new NodeIdValue(SampleDataProvider.SampleRemoteNodeKeys1.PublicKey), true,
            new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23440), null, null, false),
        new BlockchainNode(new NodeIdValue(SampleDataProvider.SampleRemoteNodeKeys1.PublicKey), true,
        new IPEndPoint(IPAddress.Parse("85.3.3.221"), 23000), new IPEndPoint(IPAddress.Parse("85.3.3.221"), 23001), new DateTime(2022,1,1), true)
    };
}