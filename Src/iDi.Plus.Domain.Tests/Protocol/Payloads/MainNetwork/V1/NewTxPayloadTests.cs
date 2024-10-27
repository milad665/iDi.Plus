using System.Collections.Generic;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using iDi.Plus.Domain.Tests.Protocol.TestData;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol.Payloads.MainNetwork.V1;

public class NewTxPayloadTests : ProtocolsTestBase, IPayloadTest
{
    [Fact]
    public void MessageCreatedSuccessfullyFromByteArray()
    {
        var transactionHashes = new List<HashValue>
        {
            TransactionTestData.SampleTransactionIdCard3PassportName1.TransactionHash,
            TransactionTestData.SampleTransactionIdCard3PassportExpirationDate1.TransactionHash,
            TransactionTestData.SampleTransactionIdCard3PassportExpirationDate2.TransactionHash,
            TransactionTestData.SampleTransactionIdCard2PassportName1.TransactionHash,
            TransactionTestData.SampleTransactionIdCard2PassportExpirationDate1.TransactionHash
        };

        var bytes = SampleDataProvider.NewTxsPayloadBytes(transactionHashes);
        var target = new NewBlocksPayload(bytes);

        Assert.Equal(transactionHashes.Count, target.Blocks.Count);
        transactionHashes.ForEach(h => Assert.Contains(h, target.Blocks));
    }

    [Fact]
    public void RawDataCreatedSuccessfully()
    {
        var transactionHashes = new List<HashValue>
        {
            TransactionTestData.SampleTransactionIdCard3PassportName1.TransactionHash,
            TransactionTestData.SampleTransactionIdCard3PassportExpirationDate1.TransactionHash,
            TransactionTestData.SampleTransactionIdCard3PassportExpirationDate2.TransactionHash,
            TransactionTestData.SampleTransactionIdCard2PassportName1.TransactionHash,
            TransactionTestData.SampleTransactionIdCard2PassportExpirationDate1.TransactionHash
        };

        var bytes = SampleDataProvider.NewTxsPayloadBytes(transactionHashes);
        var target = HotPoolTxsPayload.Create(transactionHashes);

        Assert.Equal(bytes.ToHexString(), target.RawData.ToHexString(), true);
    }
}