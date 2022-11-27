using System.ComponentModel.DataAnnotations;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol.Payloads.MainNetwork.V1;

public class VotePayloadTests: ProtocolsTestBase, IPayloadTest
{
    [Fact]
    public void MessageCreatedSuccessfullyFromByteArray()
    {
        var nodeId = SampleDataProvider.SampleRemoteNodeKeys1.PublicKey;
        var target = new VotePayload(nodeId);
        Assert.Equal(nodeId, target.NodeId.Bytes);
    }

    [Fact]
    public void RawDataCreatedSuccessfully()
    {
        var nodeId = SampleDataProvider.SampleRemoteNodeKeys1.PublicKey;
        var target = VotePayload.Create(new NodeIdValue(nodeId));
        Assert.Equal(nodeId, target.RawData);
    }
}