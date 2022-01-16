using System;
using System.Collections.Generic;
using System.Linq;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using Xunit;

namespace iDi.Plus.Domain.Tests.Protocol.Payloads.MainNetwork.V1;

public class BlockDataPayloadTests : ProtocolsTestBase, IPayloadTest
{
    [Fact]
    public void MessageCreatedSuccessfullyFromByteArray()
    {
        var block = SampleDataProvider.Block1;

        var payloadData = SampleDataProvider.BlockDataPayload(block);
        var target = new BlockDataPayload(payloadData);
        
        Assert.Equal(block.Hash, target.Hash,true);
        Assert.Equal(block.PreviousHash, target.PreviousHash, true);
        Assert.Equal(block.Nonce, target.Nonce);
        Assert.Equal(block.Index, target.Index);
        Assert.Equal(block.Transactions.Count, target.Transactions.Count);
        block.Transactions.ForEach(t => Assert.Contains(target.Transactions, tt => tt.TransactionHash.Equals(t.TransactionHash, StringComparison.InvariantCultureIgnoreCase)));
    }

    [Fact]
    public void RawDataCreatedSuccessfully()
    {
        var block = SampleDataProvider.Block1;

        var expectedPayloadData = SampleDataProvider.BlockDataPayload(block);
        var transactionsPayload = block.Transactions.Select(t => TxDataPayload.Create(t.TransactionHash,
            TransactionTypes.IssueTransaction, t.Issuer.Address, t.Holder.Address, t.Verifier?.Address, t.Subject,
            t.Identifier, t.Timestamp, t.PreviousTransactionHash, t.SignedData)).ToList();

        var target = BlockDataPayload.Create(block.Index, block.Hash, block.PreviousHash, block.TimeStamp, transactionsPayload, block.Nonce);

        Assert.Equal(expectedPayloadData.ToHexString(), target.RawData.ToHexString(), true);
    }

    [Fact]
    public void RawDataCreatedSuccessfully_ForNullTransactions()
    {
        var block = SampleDataProvider.Block1;
        block.Transactions = null;
        var expectedPayloadData = SampleDataProvider.BlockDataPayload(block);

        var target = BlockDataPayload.Create(block.Index, block.Hash, block.PreviousHash, block.TimeStamp, null, block.Nonce);

        Assert.Equal(expectedPayloadData.ToHexString(), target.RawData.ToHexString(), true);
    }

    [Fact]
    public void RawDataCreatedSuccessfully_ForEmptyTransactionsList()
    {
        var block = SampleDataProvider.Block1;
        block.Transactions = null;
        var expectedPayloadData = SampleDataProvider.BlockDataPayload(block);

        var target = BlockDataPayload.Create(block.Index, block.Hash, block.PreviousHash, block.TimeStamp, new List<TxDataPayload>(), block.Nonce);

        Assert.Equal(expectedPayloadData.ToHexString(), target.RawData.ToHexString(), true);
    }

    [Fact]
    public void ThrowError_InvalidHashLength()
    {
        var block = SampleDataProvider.Block1;

        var transactionsPayload = block.Transactions.Select(t => TxDataPayload.Create(t.TransactionHash,
            TransactionTypes.IssueTransaction, t.Issuer.Address, t.Holder.Address, t.Verifier?.Address, t.Subject,
            t.Identifier, t.Timestamp, t.PreviousTransactionHash, t.SignedData)).ToList();

        Assert.Throws<InvalidDataException>(() => BlockDataPayload.Create(block.Index, block.Hash.Substring(10), block.PreviousHash, block.TimeStamp, transactionsPayload, block.Nonce));
    }

    [Fact]
    public void ThrowError_InvalidPreviousHashLength()
    {
        var block = SampleDataProvider.Block1;

        var transactionsPayload = block.Transactions.Select(t => TxDataPayload.Create(t.TransactionHash,
            TransactionTypes.IssueTransaction, t.Issuer.Address, t.Holder.Address, t.Verifier?.Address, t.Subject,
            t.Identifier, t.Timestamp, t.PreviousTransactionHash, t.SignedData)).ToList();

        Assert.Throws<InvalidDataException>(() => BlockDataPayload.Create(block.Index, block.Hash, block.PreviousHash.Substring(10), block.TimeStamp, transactionsPayload, block.Nonce));
    }

    [Fact]
    public void ThrowError_InvalidHashFormat()
    {
        var block = SampleDataProvider.Block1;

        var transactionsPayload = block.Transactions.Select(t => TxDataPayload.Create(t.TransactionHash,
            TransactionTypes.IssueTransaction, t.Issuer.Address, t.Holder.Address, t.Verifier?.Address, t.Subject,
            t.Identifier, t.Timestamp, t.PreviousTransactionHash, t.SignedData)).ToList();

        Assert.Throws<FormatException>(() => BlockDataPayload.Create(block.Index, "INVALIDHASH", block.PreviousHash, block.TimeStamp, transactionsPayload, block.Nonce));
    }

    [Fact]
    public void ThrowError_InvalidPreviousHashFormat()
    {
        var block = SampleDataProvider.Block1;

        var transactionsPayload = block.Transactions.Select(t => TxDataPayload.Create(t.TransactionHash,
            TransactionTypes.IssueTransaction, t.Issuer.Address, t.Holder.Address, t.Verifier?.Address, t.Subject,
            t.Identifier, t.Timestamp, t.PreviousTransactionHash, t.SignedData)).ToList();

        Assert.Throws<FormatException>(() => BlockDataPayload.Create(block.Index, block.Hash, "INVALIDHASH", block.TimeStamp, transactionsPayload, block.Nonce));
    }
}