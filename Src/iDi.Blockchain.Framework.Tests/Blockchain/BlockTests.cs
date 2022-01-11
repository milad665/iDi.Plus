using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using iDi.Blockchain.Framework.Blockchain;
using Xunit;

namespace iDi.Blockchain.Framework.Tests.Blockchain;

public class BlockTests
{
    [Fact]
    public void GenesisBlockIsValid()
    {
        var genesis = Block<TestTransaction>.Genesis();

        Assert.Equal(0, genesis.Index);
        Assert.Equal("", genesis.PreviousHash);
        Assert.Null(genesis.Transactions);
    }

    [Fact]
    public void NextNonceProducesCorrectHash()
    {
        var genesis = Block<TestTransaction>.Genesis();
        var block = new Block<TestTransaction>(1, genesis.Hash, DateTime.UtcNow,
            new List<TestTransaction> {new TestTransaction(TransactionTypes.IssueTransaction, "Value")});
        
        Assert.Equal(0, block.Nonce);
        block.NextNonce();
        Assert.Equal(1, block.Nonce);
        var hash = ComputeHash(block);
        Assert.Equal(hash, block.Hash);
    }

    private string ComputeHash(Block<TestTransaction> data)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
        using var algorithm = FrameworkEnvironment.HashAlgorithm;
        var hashedBytes = algorithm.ComputeHash(bytes);
        return Encoding.UTF8.GetString(hashedBytes);
    }
}