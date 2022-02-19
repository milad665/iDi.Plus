using System.Collections.Generic;
using iDi.Blockchain.Framework.Exceptions;

namespace iDi.Blockchain.Framework.Blockchain;

/// <summary>
/// Abstracts the main logic behind a blockchain
/// </summary>
/// <typeparam name="TTransaction"></typeparam>
public interface IBlockchain<TTransaction> where TTransaction : ITransaction
{
    /// <summary>
    /// Last block of the blockchain
    /// </summary>
    Block<TTransaction> LastBlock { get; }

    /// <summary>
    /// Number of blocks in the blockchain
    /// </summary>
    long BlocksCount { get; }

    /// <summary>
    /// Current difficulty level of the PoW algorithm
    /// </summary>
    int Difficulty { get; }

    /// <summary>
    /// Create a block from a list of transactions and adds it to the blockchain
    /// This method verifies all the transactions, creates the block and executes PoW on it and finally adds the block to the blockchain.
    /// </summary>
    /// <param name="transactions">Transactions to be sealed in a block</param>
    /// <returns>The newly created block instance</returns>
    Block<TTransaction> AddNewBlock(List<TTransaction> transactions);

    /// <summary>
    /// Verifies a block by verifying its hash, its index and its reference to the previous block
    /// This method does NOT verify the transactions within a block
    /// </summary>
    /// <param name="block"></param>
    /// <exception cref="VerificationFailedException"></exception>
    void VerifyBlock(Block<TTransaction> block);

    /// <summary>
    /// Verifies a transaction of type TTransaction
    /// </summary>
    /// <param name="transaction">The transaction to be verified</param>
    void VerifyTransaction(TTransaction transaction);
}