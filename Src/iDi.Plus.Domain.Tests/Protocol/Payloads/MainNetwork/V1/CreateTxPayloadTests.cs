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
        Assert.Equal(txData.TransactionHash, target.TxDataPayload.TransactionHash);
        Assert.Equal(txData.PreviousTransactionHash, target.TxDataPayload.PreviousTransactionHash);
        Assert.Equal(txData.Timestamp, target.TxDataPayload.Timestamp);
        Assert.Equal(txData.Subject, target.TxDataPayload.Subject);
        Assert.Equal(txData.SignedData, target.TxDataPayload.DoubleEncryptedData);
        Assert.Equal(txData.Identifier, target.TxDataPayload.IdentifierKey);
        Assert.Equal(txData.Holder.Address, target.TxDataPayload.HolderAddress);
        Assert.Equal(txData.Issuer.Address, target.TxDataPayload.IssuerAddress);
        Assert.Equal(txData.Verifier?.Address ?? AddressValue.Empty, target.TxDataPayload.VerifierAddress);
    }

    [Fact]
    public void RawDataCreatedSuccessfully()
    {
        var txData = TransactionTestData.SampleTransactionIdCard2PassportName1;
        var txDataPayloadBytes = SampleDataProvider.TxDataPayloadBytes(txData);
        var createTransactionBytes = SampleDataProvider.CreateTxPayloadBytes(txData);
        var target = CreateTxPayload.FromTxDataPayload(txData.Issuer, new TxDataPayload(txDataPayloadBytes));

        Assert.Equal(createTransactionBytes.ToHexString(), target.RawData.ToHexString(), true);
    }
}