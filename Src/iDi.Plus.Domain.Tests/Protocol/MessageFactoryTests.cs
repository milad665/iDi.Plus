using System;
using iDi.Blockchain.Framework.Protocol;
using iDi.Plus.Domain.Protocol;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol;

public class MessageFactoryTests
{
    private readonly SampleDataProvider _smSampleDataProvider;

    public MessageFactoryTests()
    {
        _smSampleDataProvider = new SampleDataProvider();
    }

    [Fact]
    public void MessageCreatedSuccessfullyFromByteArray()
    {
        var target = new MessageFactory();

        var message = target.CreateMessage(_smSampleDataProvider.BlockDataMessageBytes(true, Networks.Main, 1));
        Assert.Equal(MessageTypes.BlockData, message.Header.MessageType);
    }

    [Fact]
    public void ThrowsError_OnMessageCreationFromBytes_WhenSignatureIsNotValid()
    {
        var target = new MessageFactory();

        Assert.Throws<UnauthorizedAccessException>(() => target.CreateMessage(_smSampleDataProvider.BlockDataMessageBytes(false, Networks.Main, 1)));
    }

    [Fact]
    public void ThrowsError_OnMessageCreationFromBytes_WhenNetworkNotMain()
    {
        var target = new MessageFactory();

        Assert.Throws<NotSupportedException>(() => target.CreateMessage(_smSampleDataProvider.BlockDataMessageBytes(false, Networks.TestNet1, 1)));
    }

    [Fact]
    public void ThrowsError_OnMessageCreationFromBytes_WhenVersionNotOne()
    {
        var target = new MessageFactory();

        Assert.Throws<NotSupportedException>(() => target.CreateMessage(_smSampleDataProvider.BlockDataMessageBytes(false, Networks.Main, 2)));
    }
}