﻿using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol.Payloads.MainNetwork.V1;

public class TxDataPayloadTests : ProtocolsTestBase, IPayloadTest
{
    [Fact]
    public void MessageCreatedSuccessfullyFromByteArray()
    {
        var txData = SampleDataProvider.Transaction1;
        var transactionBytes = SampleDataProvider.TransactionDataPayloadBytes(txData);
        var target = new TxDataPayload(transactionBytes);

        Assert.Equal(txData.TransactionHash, target.TransactionHash, true);
        Assert.Equal(txData.PreviousTransactionHash, target.PreviousTransactionHash, true);
        Assert.Equal(txData.Timestamp, target.Timestamp);
        Assert.Equal(txData.Subject, target.Subject);
        Assert.Equal(txData.SignedData, target.SignedData);
        Assert.Equal(txData.Identifier, target.IdentifierKey);
        Assert.Equal(txData.Holder.Address, target.HolderAddress,true);
        Assert.Equal(txData.Issuer.Address, target.IssuerAddress,true);
        Assert.Equal(txData.Verifier?.Address, target.VerifierAddress,true);
    }

    [Fact]
    public void RawDataCreatedSuccessfully()
    {
        var txData = SampleDataProvider.Transaction1;
        var transactionBytes = SampleDataProvider.TransactionDataPayloadBytes(txData);
        var target = TxDataPayload.Create(txData.TransactionHash, TransactionTypes.IssueTransaction,
            txData.Issuer.Address, txData.Holder.Address, txData.Verifier?.Address, txData.Subject, txData.Identifier,
            txData.Timestamp, txData.PreviousTransactionHash, txData.SignedData);

        Assert.Equal(transactionBytes.ToHexString(), target.RawData.ToHexString(),true);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("Address")]
    [InlineData("162fe433afd216")] //Too small
    public void ThrowsError_InvalidIssueAddress(string invalidIssuerAddress)
    {
        var txData = SampleDataProvider.Transaction1;
        Assert.Throws<InvalidDataException>(()=>TxDataPayload.Create(txData.TransactionHash, TransactionTypes.IssueTransaction,
            invalidIssuerAddress, txData.Holder.Address, txData.Verifier?.Address, txData.Subject, txData.Identifier,
            txData.Timestamp, txData.PreviousTransactionHash, txData.SignedData));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("Address")]
    [InlineData("162fe433afd216")] //Too small
    public void ThrowsError_InvalidHolderAddress(string invalidHolderAddress)
    {
        var txData = SampleDataProvider.Transaction1;
        Assert.Throws<InvalidDataException>(() => TxDataPayload.Create(txData.TransactionHash, TransactionTypes.IssueTransaction,
            txData.Issuer.Address, invalidHolderAddress, txData.Verifier?.Address, txData.Subject, txData.Identifier,
            txData.Timestamp, txData.PreviousTransactionHash, txData.SignedData));
    }
}