using System;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol.Payloads.MainNetwork.V1;

public class GetNewBlocksPayloadTests : ProtocolsTestBase
{
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(1000000)]
    [InlineData(long.MaxValue)]
    public void MessageCreatedSuccessfullyFromByteArray(long lastBlockIndex)
    {
        var bytes = SampleDataProvider.GetNewBlocksPayloadBytes(lastBlockIndex);
        var target = new RequestBlockchainUpdatePayload(bytes);

        Assert.Equal(lastBlockIndex, target.LastBlockIndex);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(1000000)]
    [InlineData(long.MaxValue)]
    public void RawDataCreatedSuccessfully(long lastBlockIndex)
    {
        var bytes = SampleDataProvider.GetNewBlocksPayloadBytes(lastBlockIndex);
        var target = RequestBlockchainUpdatePayload.Create(lastBlockIndex);

        Assert.Equal(bytes.ToHexString(), target.RawData.ToHexString(),true);
    }

    [Fact]
    public void ThrowsError_WhenLastBlockIndexIsNegative()
    {
        var lastBlockIndex = -1;
        Assert.Throws<InvalidInputException>(() => RequestBlockchainUpdatePayload.Create(lastBlockIndex));
    }

    [Fact]
    public void ThrowsError_WhenRawDataContainsNegativeLastBlockIndex()
    {
        var lastBlockIndex = -1;
        var bytes = SampleDataProvider.GetNewBlocksPayloadBytes(lastBlockIndex);
        Assert.Throws<InvalidInputException>(() => new RequestBlockchainUpdatePayload(bytes));
    }

    [Fact]
    public void ThrowsError_WhenRawDataLengthIsIncorrect()
    {
        var bytes = BitConverter.GetBytes(1); //int = 4 bytes
        Assert.Throws<InvalidInputException>(() => new RequestBlockchainUpdatePayload(bytes));
    }
}