using System;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Exceptions;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;

namespace iDi.Plus.Domain.Services;

public class VerificationService : IVerificationService
{
    private readonly IIdBlockchainRepository _blockchainRepository;

    public VerificationService(IIdBlockchainRepository blockchainRepository)
    {
        _blockchainRepository = blockchainRepository;
    }

    public void VerifyBlock(Block<IdTransaction> block)
    {
        block.VerifyHash();
        var lastBlock = _blockchainRepository.GetLastBlock();
        if (lastBlock.Hash != block.PreviousHash)
            throw new VerificationFailedException("Previous hash does not match the hash of the previous block.");
        if (block.Index != lastBlock.Index + 1)
            throw new VerificationFailedException("Invalid block index.");

        block.Transactions.ForEach(VerifyTransaction);
    }

    public void VerifyTransaction(IdTransaction transaction)
    {
        //var lastTransaction = _blockchainRepository.
        //transaction.VerifyHash();
        throw new NotImplementedException();
    }
}