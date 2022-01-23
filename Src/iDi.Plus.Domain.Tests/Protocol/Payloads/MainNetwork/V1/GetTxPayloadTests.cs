using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using iDi.Plus.Domain.Tests.Protocol.TestData;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol.Payloads.MainNetwork.V1;

public class GetTxPayloadTests : ProtocolsTestBase, IPayloadTest
{
    [Fact]
    public void MessageCreatedSuccessfullyFromByteArray()
    {
        var transactionHash = TransactionTestData.SampleTransactionIdCard2PassportName1.TransactionHash;
        var bytes = SampleDataProvider.GetTxPayloadBytes(transactionHash);
        var target = new GetTxPayload(bytes);

        Assert.Equal(transactionHash, target.TransactionHash);
    }

    [Fact]
    public void RawDataCreatedSuccessfully()
    {
        var transactionHash = TransactionTestData.SampleTransactionIdCard2PassportName1.TransactionHash;
        var target = GetTxPayload.Create(transactionHash);

        Assert.Equal(transactionHash.Bytes.ToHexString(), target.RawData.ToHexString(), true);
    }
}