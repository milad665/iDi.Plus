using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Exceptions;

namespace iDi.Blockchain.Framework.Blockchain;

/// <summary>
/// Encapsulates a single block in a blockchain of transaction type TTransaction
/// </summary>
/// <typeparam name="TTransaction">Type of transaction that the block contains</typeparam>
public class Block<TTransaction> where TTransaction : ITransaction
{
    private Block()
    {}

    public Block(long index, HashValue hash, HashValue previousHash, DateTime timestamp, IEnumerable<TTransaction> transactions, long nonce)
    {
        Index = index;
        Hash = hash;
        PreviousHash = previousHash;
        Timestamp = timestamp;
        Transactions = transactions;
        Nonce = nonce;
    }

    private Block(long index, HashValue previousHash, DateTime timestamp, IEnumerable<TTransaction> transactions)
    {
        Index = index;
        PreviousHash = previousHash;
        Timestamp = timestamp;
        Transactions = transactions;
        Nonce = 0;
        Hash = GetHash();
    }

    
    /// <summary>
    /// Creates a new block in the blockchain containing the provided list of transactions
    /// </summary>
    /// <param name="index">Block index in the blockchain</param>
    /// <param name="previousHash">Hash of the previous block in the blockchain</param>
    /// <param name="timestamp">Timestamp of the block</param>
    /// <param name="transactions">List of transactions to be wrapped by the block</param>
    /// <returns>New block instance</returns>
    public static Block<TTransaction> Create(long index, HashValue previousHash, DateTime timestamp, IEnumerable<TTransaction> transactions)
    {
        return new Block<TTransaction>(index, previousHash, timestamp, transactions);
    }

    /// <summary>
    /// Creates the genesis block. Genesis block is the first block in the blockchain with index 0 and no transactions.
    /// </summary>
    /// <returns>Genesis block</returns>
    public static Block<TTransaction> Genesis()
    {
        return new Block<TTransaction>(0, HashValue.Empty, DateTime.UtcNow, null);
    }
    
    /// <summary>
    /// Block index
    /// </summary>
    public long Index { get; private set; }
    
    /// <summary>
    /// Block hash
    /// </summary>
    [JsonIgnore]
    public HashValue Hash { get; private set; }
    
    /// <summary>
    /// Hash of the previous block in the blockchain
    /// </summary>
    public HashValue PreviousHash { get; private set; }
    
    /// <summary>
    /// Timestamp of the block
    /// </summary>
    public DateTime Timestamp { get; private set; }
    
    /// <summary>
    /// Transactions contained in the block
    /// </summary>
    public IEnumerable<TTransaction> Transactions { get; private set; }
    
    /// <summary>
    /// Nonce of the block
    /// </summary>
    public long Nonce { get; private set; }

    /// <summary>
    /// Checks whether the current block is genesis block or not
    /// </summary>
    /// <returns>True if the block is genesis block</returns>
    public bool IsGenesis() => Index == 0 && PreviousHash.IsEmpty() && Transactions == null;

    /// <summary>
    /// Increments the nonce
    /// </summary>
    public void NextNonce()
    {
        Nonce++;
        Hash = GetHash();
    }

    /// <summary>
    /// Verifies if the block hash matches the hash of the block data.
    /// </summary>
    /// <exception cref="VerificationFailedException">Thrown when the hash is invalid</exception>
    public void VerifyHash()
    {
        var computedHash = GetHash();
        if (Hash != computedHash)
            throw new VerificationFailedException("Invalid block hash.");
    }

    private HashValue GetHash()
    {
        return HashValue.ComputeHash(this);
    }
}