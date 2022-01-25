using System.Collections.Generic;
using System.Linq;
using System.Net;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using iDi.Plus.Domain.Protocol.Processors;
using iDi.Plus.Domain.Tests.Protocol.TestData;
using Moq;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol.Processors;

public class NewBlocksMessageProcessorTests : MessageProcessorTestBase
{
    private NewBlocksMessageProcessor Target => new(BlockchainNodeClientMock.Object, BlockchainRepositoryMock.Object,
        LocalNodeContextProviderMock.Object, BlockchainNodesProviderMock.Object);

    [Fact]
    public void ResponseMessageCreatedSuccessfully()
    {
        var dicCalls = new Dictionary<Message, IPEndPoint>();

        BlockchainNodeClientMock.Setup(c => c.Send(It.IsAny<IPEndPoint>(), It.IsAny<Message>()))
            .Callback((IPEndPoint ip, Message message) => dicCalls.Add(message, ip));

        var newBlockHashes = new List<HashValue>
        {
            BlockTestData.SampleBlock1.Hash,
            BlockTestData.SampleBlock2.Hash,
            BlockTestData.SampleBlock3.Hash,
            BlockTestData.SampleBlock4.Hash,
        };
        var message = SampleDataProvider.NewBlocksMessage(newBlockHashes);
        var responseMessage = Target.ProcessPayload(message);

        Assert.Null(responseMessage);

        //Block request messages are successfully sent back
        Assert.Equal(newBlockHashes.Count, dicCalls.Count);
        var actualSentPayloads = dicCalls.Keys.Select(k => k.Payload).Cast<GetBlockPayload>();
        newBlockHashes.ForEach(h =>
            Assert.Contains(h, actualSentPayloads.Select(p => p.BlockHash)));
        
        dicCalls.Keys.ToList().ForEach(m => Assert.Equal(RemoteNodes[message.Header.NodeId].VerifiedEndpoint1,dicCalls[m]));
    }

    [Fact]
    public void ThrowsErrorOnInvalidInputMessageType()
    {
        var message = SampleDataProvider.GetTxMessage(TransactionTestData.SampleTransactionIdCard3PassportName1.TransactionHash);
        Assert.Throws<InvalidInputException>(() => Target.ProcessPayload(message));
    }
}