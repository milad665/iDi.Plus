﻿using System;
using System.Collections.Generic;
using System.Linq;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using iDi.Plus.Domain.Tests.Protocol.TestData;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol.Payloads.MainNetwork.V1;

public class BlockDataPayloadTests : ProtocolsTestBase, IPayloadTest
{
    [Fact]
    public void MessageCreatedSuccessfullyFromByteArray()
    {
        var block = BlockTestData.SampleBlock1;

        var payloadData = SampleDataProvider.BlockDataPayloadBytes(block);
        var target = new BlockDataPayload(payloadData);
        
        Assert.Equal(block.Hash, target.Hash);
        Assert.Equal(block.PreviousHash, target.PreviousHash);
        Assert.Equal(block.Nonce, target.Nonce);
        Assert.Equal(block.Index, target.Index);
        Assert.Equal(block.Transactions.Count, target.Transactions.Count);
        block.Transactions.ForEach(t => Assert.Contains(target.Transactions, tt => tt.TransactionHash.Equals(t.TransactionHash)));
    }

    [Fact]
    public void RawDataCreatedSuccessfully()
    {
        var block = BlockTestData.SampleBlock1;

        var expectedPayloadData = SampleDataProvider.BlockDataPayloadBytes(block);
        var transactionsPayload = block.Transactions.Select(t => TxDataResponsePayload.Create(t.TransactionHash,
            TransactionTypes.IssueTransaction, t.Issuer.Address, t.Holder.Address, t.Verifier?.Address, t.Subject,
            t.Identifier, t.Timestamp, t.PreviousTransactionHash,"text/plain", t.SignedData)).ToList();

        var target = BlockDataPayload.Create(block.Index, block.Hash, block.PreviousHash, block.TimeStamp, transactionsPayload, block.Nonce);

        Assert.Equal(expectedPayloadData.ToHexString(), target.RawData.ToHexString(), true);
    }

    [Fact]
    public void RawDataCreatedSuccessfully_ForNullTransactions()
    {
        var block = BlockTestData.SampleBlock1;
        block.Transactions = null;
        var expectedPayloadData = SampleDataProvider.BlockDataPayloadBytes(block);

        var target = BlockDataPayload.Create(block.Index, block.Hash, block.PreviousHash, block.TimeStamp, null, block.Nonce);

        Assert.Equal(expectedPayloadData.ToHexString(), target.RawData.ToHexString(), true);
    }

    [Fact]
    public void RawDataCreatedSuccessfully_ForEmptyTransactionsList()
    {
        var block = BlockTestData.SampleBlock1;
        block.Transactions = null;
        var expectedPayloadData = SampleDataProvider.BlockDataPayloadBytes(block);

        var target = BlockDataPayload.Create(block.Index, block.Hash, block.PreviousHash, block.TimeStamp, new List<TxDataResponsePayload>(), block.Nonce);

        Assert.Equal(expectedPayloadData.ToHexString(), target.RawData.ToHexString(), true);
    }

    [Fact]
    public void ThrowError_InvalidHashLength()
    {
        var block = BlockTestData.SampleBlock1;

        var transactionsPayload = block.Transactions.Select(t => TxDataResponsePayload.Create(t.TransactionHash,
            TransactionTypes.IssueTransaction, t.Issuer.Address, t.Holder.Address, t.Verifier?.Address, t.Subject,
            t.Identifier, t.Timestamp, t.PreviousTransactionHash,"text/plain", t.SignedData)).ToList();

        Assert.Throws<InvalidInputException>(() => BlockDataPayload.Create(block.Index, new HashValue("ab12"), block.PreviousHash, block.TimeStamp, transactionsPayload, block.Nonce));
    }

    [Fact]
    public void ThrowError_InvalidPreviousHashLength()
    {
        var block = BlockTestData.SampleBlock1;

        var transactionsPayload = block.Transactions.Select(t => TxDataResponsePayload.Create(t.TransactionHash,
            TransactionTypes.IssueTransaction, t.Issuer.Address, t.Holder.Address, t.Verifier?.Address, t.Subject,
            t.Identifier, t.Timestamp, t.PreviousTransactionHash,"text/plain", t.SignedData)).ToList();

        Assert.Throws<InvalidInputException>(() => BlockDataPayload.Create(block.Index, block.Hash, new HashValue("ab12"), block.TimeStamp, transactionsPayload, block.Nonce));
    }

    [Fact]
    public void ThrowError_InvalidHashFormat()
    {
        var block = BlockTestData.SampleBlock1;

        var transactionsPayload = block.Transactions.Select(t => TxDataResponsePayload.Create(t.TransactionHash,
            TransactionTypes.IssueTransaction, t.Issuer.Address, t.Holder.Address, t.Verifier?.Address, t.Subject,
            t.Identifier, t.Timestamp, t.PreviousTransactionHash,"text/plain", t.SignedData)).ToList();

        Assert.Throws<FormatException>(() => BlockDataPayload.Create(block.Index, new HashValue("INVALIDHASH"), block.PreviousHash, block.TimeStamp, transactionsPayload, block.Nonce));
    }

    [Fact]
    public void ThrowError_InvalidPreviousHashFormat()
    {
        var block = BlockTestData.SampleBlock1;

        var transactionsPayload = block.Transactions.Select(t => TxDataResponsePayload.Create(t.TransactionHash,
            TransactionTypes.IssueTransaction, t.Issuer.Address, t.Holder.Address, t.Verifier?.Address, t.Subject,
            t.Identifier, t.Timestamp, t.PreviousTransactionHash,"text/plain", t.SignedData)).ToList();

        Assert.Throws<FormatException>(() => BlockDataPayload.Create(block.Index, block.Hash, new HashValue("INVALIDHASH"), block.TimeStamp, transactionsPayload, block.Nonce));
    }
}