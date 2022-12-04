using System.Linq;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using iDi.Plus.Domain.Tests.Protocol.TestData;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol.Payloads.MainNetwork.V1;

public class CreateTxPayloadTests : ProtocolsTestBase, IPayloadTest
{
    [Fact]
    public void MessageCreatedSuccessfullyFromByteArray()
    {
        var txData = TransactionTestData.SampleTransactionIdCard2PassportName1;
        var transactionBytes = SampleDataProvider.CreateTxPayloadBytes(txData);
        var target = new CreateTxPayload(transactionBytes);

        Assert.Equal(txData.Issuer.PublicKey.ToHexString(), target.SenderPublicKey.ToHexString(),true);
        Assert.Equal(transactionBytes.Skip(txData.Issuer.PublicKey.Length).ToArray().ToHexString(), target.EncryptedTransactionData.ToHexString(),true);
    }

    [Fact]
    public void RawDataCreatedSuccessfully()
    {
        var txData = TransactionTestData.SampleTransactionIdCard2PassportName1;
        var txDataPayloadBytes = SampleDataProvider.TxDataPayloadBytes(txData);
        var createTransactionBytes = SampleDataProvider.CreateTxPayloadBytes(txData);
        var target = CreateTxPayload.FromTxDataPayload(txData.Issuer, new TxDataResponsePayload(txDataPayloadBytes));

        Assert.Equal(createTransactionBytes.ToHexString(), target.RawData.ToHexString(), true);
    }
}