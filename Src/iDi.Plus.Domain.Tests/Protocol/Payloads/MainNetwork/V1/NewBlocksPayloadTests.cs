using System.Collections.Generic;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using iDi.Plus.Domain.Tests.Protocol.TestData;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol.Payloads.MainNetwork.V1;

public class NewBlocksPayloadTests : ProtocolsTestBase, IPayloadTest
{
    [Fact]
    public void MessageCreatedSuccessfullyFromByteArray()
    {
        var blockHashes = new List<HashValue>
        {
            BlockTestData.SampleBlock1.Hash,
            BlockTestData.SampleBlock2.Hash,
            BlockTestData.SampleBlock3.Hash,
            BlockTestData.SampleBlock4.Hash
        };

        var bytes = SampleDataProvider.NewBlocksPayloadBytes(blockHashes);
        var target = new NewBlocksPayload(bytes);

        Assert.Equal(blockHashes.Count, target.Blocks.Count);
        blockHashes.ForEach(h => Assert.Contains(h, target.Blocks));
    }

    [Fact]
    public void RawDataCreatedSuccessfully()
    {
        var blockHashes = new List<HashValue>
        {
            BlockTestData.SampleBlock1.Hash,
            BlockTestData.SampleBlock2.Hash,
            BlockTestData.SampleBlock3.Hash,
            BlockTestData.SampleBlock4.Hash
        };

        var bytes = SampleDataProvider.NewBlocksPayloadBytes(blockHashes);
        var target = NewBlocksPayload.Create(blockHashes);

        Assert.Equal(bytes.ToHexString(), target.RawData.ToHexString(),true);
    }
}