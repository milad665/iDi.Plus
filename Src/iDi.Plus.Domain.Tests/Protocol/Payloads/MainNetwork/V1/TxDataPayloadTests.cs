﻿using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using iDi.Plus.Domain.Tests.Protocol.TestData;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol.Payloads.MainNetwork.V1;

public class TxDataPayloadTests : ProtocolsTestBase, IPayloadTest
{
    [Fact]
    public void MessageCreatedSuccessfullyFromByteArray()
    {
        var txData = TransactionTestData.SampleTransactionIdCard2PassportName1;
        var transactionBytes = SampleDataProvider.TxDataPayloadBytes(txData);
        var target = new TxDataResponsePayload(transactionBytes);

        Assert.Equal(txData.TransactionHash, target.TransactionHash);
        Assert.Equal(txData.PreviousTransactionHash, target.PreviousTransactionHash);
        Assert.Equal(txData.Timestamp, target.Timestamp);
        Assert.Equal(txData.Subject, target.Subject);
        Assert.Equal(txData.SignedData, target.Value);
        Assert.Equal(txData.Identifier, target.IdentifierKey);
        Assert.Equal(txData.Holder.Address, target.HolderAddress);
        Assert.Equal(txData.Issuer.Address, target.IssuerAddress);
        Assert.Equal(txData.Verifier?.Address ?? AddressValue.Empty, target.VerifierAddress);
    }

    [Fact]
    public void RawDataCreatedSuccessfully()
    {
        var txData = TransactionTestData.SampleTransactionIdCard2PassportName1;
        var transactionBytes = SampleDataProvider.TxDataPayloadBytes(txData);
        var target = TxDataResponsePayload.Create(txData.TransactionHash, TransactionTypes.IssueTransaction,
            txData.Issuer.Address, txData.Holder.Address, txData.Verifier?.Address, txData.Subject, txData.Identifier,
            txData.Timestamp, txData.PreviousTransactionHash,"text/plain", txData.SignedData);

        Assert.Equal(transactionBytes.ToHexString(), target.RawData.ToHexString(),true);
    }
}