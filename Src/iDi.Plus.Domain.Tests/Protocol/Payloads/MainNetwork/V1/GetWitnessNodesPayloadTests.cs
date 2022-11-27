using System;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol.Payloads.MainNetwork.V1;

public class GetWitnessNodesPayloadTests: ProtocolsTestBase
{
    [Theory]
    [InlineData(null)]
    [InlineData(new byte[]{})]
    public void MessageCreatedSuccessfullyFromByteArray(byte[] rawData)
    {
        var target = new GetWitnessNodesPayload(rawData);
        Assert.Empty(target.RawData);
        Assert.NotNull(target.RawData);
    }
    
    [Fact]
    public void UnEmptyDataThrowsError()
    {
        Assert.Throws<InvalidInputException>(()=>new GetWitnessNodesPayload(new byte[] { 0 }));
    }

    [Fact]
    public void RawDataCreatedSuccessfully()
    {
        var target = GetWitnessNodesPayload.Create();
        Assert.Empty(target.RawData);
        Assert.NotNull(target.RawData);
    }
}