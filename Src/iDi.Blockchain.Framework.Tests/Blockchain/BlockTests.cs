using System;
using System.Collections.Generic;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Cryptography;
using Xunit;

namespace iDi.Blockchain.Framework.Tests.Blockchain;

public class BlockTests
{
    [Fact]
    public void GenesisBlockIsValid()
    {
        var genesis = Block<TestTransaction>.Genesis();

        Assert.Equal(0, genesis.Index);
        Assert.Equal(HashValue.Empty, genesis.PreviousHash);
        Assert.Null(genesis.Transactions);
    }

    [Fact]
    public void NextNonceProducesCorrectHash()
    {
        var genesis = Block<TestTransaction>.Genesis();
        var block = Block<TestTransaction>.Create(1, genesis.Hash, DateTime.UtcNow,
            new List<TestTransaction> {new TestTransaction(TransactionTypes.IssueTransaction, "Value")});
        
        Assert.Equal(0, block.Nonce);
        block.NextNonce();
        Assert.Equal(1, block.Nonce);
        var hash = ComputeHash(block);
        Assert.Equal(hash, block.Hash);
    }

    private HashValue ComputeHash(Block<TestTransaction> data)
    {
        return HashValue.ComputeHash(data);
    }
}