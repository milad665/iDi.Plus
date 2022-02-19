using System;
using System.Collections.Generic;
using iDi.Blockchain.Framework.Exceptions;

namespace iDi.Blockchain.Framework.Blockchain;

/// <inheritdoc />
public abstract class BlockchainBase<TTransaction> : IBlockchain<TTransaction> where TTransaction: ITransaction
{
    private readonly IBlockchainRepository<TTransaction> _repository;

    protected BlockchainBase(IBlockchainRepository<TTransaction> repository)
    {
        _repository = repository;
    }

    public Block<TTransaction> LastBlock => _repository.GetLastBlock();
    public long BlocksCount => _repository.GetBlocksCount();

    public Block<TTransaction> AddNewBlock(List<TTransaction> transactions)
    {
        foreach (var tx in transactions)
            VerifyTransaction(tx);

        var block = Block<TTransaction>.Create(_repository.GetBlocksCount(), LastBlock.Hash, DateTime.UtcNow, transactions);
        ProofOfWork(block);

        _repository.AddBlock(block);
        return block;
    }

    public void VerifyBlock(Block<TTransaction> block)
    {
        block.VerifyHash();
        var lastBlock = _repository.GetLastBlock();
        if (lastBlock.Hash != block.PreviousHash)
            throw new VerificationFailedException("Previous hash does not match the hash of the previous block.");
        if (block.Index != lastBlock.Index + 1)
            throw new VerificationFailedException("Invalid block index.");
    }

    /// <summary>
    /// When implemented in a derived class, this method executes the PoW (Proof Of Work) algorithm on a block
    /// </summary>
    /// <param name="block">The block to perform PoW on</param>
    protected abstract void ProofOfWork(Block<TTransaction> block);
    
    /// <summary>
    /// When implemented in a derived class, this method verifies a transaction of type TTransaction
    /// </summary>
    /// <param name="transaction">The transaction to be verified</param>
    public abstract void VerifyTransaction(TTransaction transaction);

    public abstract int Difficulty { get; protected set; }
}