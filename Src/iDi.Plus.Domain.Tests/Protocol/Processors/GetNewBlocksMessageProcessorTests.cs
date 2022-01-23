using System;
using System.Collections.Generic;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using iDi.Plus.Domain.Protocol.Processors;
using iDi.Plus.Domain.Tests.Protocol.TestData;
using Moq;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol.Processors;

public class GetNewBlocksMessageProcessorTests : MessageProcessorTestBase
{
    private GetNewBlocksMessageProcessor Target => new(BlockchainNodeClientMock.Object,
        BlockchainRepositoryMock.Object,
        LocalNodeContextProviderMock.Object, BlockchainNodesProviderMock.Object);

    public GetNewBlocksMessageProcessorTests()
    {
        Setup();    
    }

    private readonly List<HashValue> _returnedTransactionsFromRepository = new List<HashValue>
    {
        TransactionTestData.SampleTransactionIdCard3PassportName1.TransactionHash,
        TransactionTestData.SampleTransactionIdCard2PassportName1.TransactionHash,
        TransactionTestData.SampleTransactionIdCard2PassportExpirationDate1.TransactionHash
    };

    private void Setup()
    {
        BlockchainRepositoryMock.Setup(r => r.GetHashesOfBlocksCreatedAfter(It.IsAny<long>()))
            .Returns(_returnedTransactionsFromRepository);
    }

    [Fact]
    public void ResponseMessageCreatedSuccessfully()
    {
        var message = SampleDataProvider.GetNewBlocksMessage(1);
        var responseMessage = Target.ProcessPayload(message);
        Assert.Equal(MessageTypes.NewBlocks, responseMessage.Header.MessageType);
        Assert.IsType<NewBlocksPayload>(responseMessage.Payload);

        var payload = (NewBlocksPayload) responseMessage.Payload;
        Assert.Equal(_returnedTransactionsFromRepository.Count, payload.Blocks.Count);
        _returnedTransactionsFromRepository.ForEach(h => Assert.Contains(h, payload.Blocks));
    }

    [Fact]
    public void ThrowsErrorOnInvalidInputMessageType()
    {
        var message = SampleDataProvider.GetTxMessage(TransactionTestData.SampleTransactionIdCard3PassportName1.TransactionHash);
        Assert.Throws<InvalidInputException>(() => Target.ProcessPayload(message));
    }
}