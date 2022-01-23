using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using iDi.Plus.Domain.Tests.Protocol.TestData;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol.Payloads.MainNetwork.V1;

public class GetBlockPayloadTests : ProtocolsTestBase, IPayloadTest
{
    [Fact]
    public void MessageCreatedSuccessfullyFromByteArray()
    {
        var blockHash = BlockTestData.SampleBlock1.Hash;
        var bytes = SampleDataProvider.GetBlockPayloadBytes(blockHash);
        var target = new GetBlockPayload(bytes);

        Assert.Equal(blockHash, target.BlockHash);
    }

    [Fact]
    public void RawDataCreatedSuccessfully()
    {
        var blockHash = BlockTestData.SampleBlock1.Hash;
        var target = GetBlockPayload.Create(blockHash);

        Assert.Equal(blockHash.Bytes.ToHexString(), target.RawData.ToHexString(), true);
    }
}