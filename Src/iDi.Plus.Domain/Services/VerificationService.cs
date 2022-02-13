using System;
using System.Collections.Generic;
using System.Linq;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Cryptography;
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

        foreach (var tx in block.Transactions)
            VerifyTransaction(tx);
    }

    public void VerifyTransaction(IdTransaction transaction)
    {
        transaction.VerifyHash();

        if (_blockchainRepository.IsObsolete(transaction.IssuerAddress))
            throw new VerificationFailedException("Issuer address is obsolete and no longer valid.");

        if (_blockchainRepository.IsObsolete(transaction.HolderAddress))
            throw new VerificationFailedException("Holder address is obsolete and no longer valid.");

        var issuerAddresses = GetPreviousAddressesOfTheIdCardAddress(transaction.IssuerAddress);
        var holderAddresses = GetPreviousAddressesOfTheIdCardAddress(transaction.HolderAddress);

        if (transaction.PreviousTransactionHash.IsEmpty())
        {
            var lastTransaction =
                _blockchainRepository.GetLastIssueTransactionInTheVirtualTransactionChainIncludingAddressChanges(
                    issuerAddresses, holderAddresses, transaction.Subject, transaction.IdentifierKey);

            if (lastTransaction != null)
                throw new VerificationFailedException(
                    "Previous transaction hash field is empty but another transaction with the same Issuer, Holder, Subject and Identifier exists.");
        }
        else
        {
            var prevTx = _blockchainRepository.GetTransaction(transaction.PreviousTransactionHash);
            if (prevTx == null)
                throw new VerificationFailedException(
                    "Invalid previous transaction. Previous transaction provided in this transaction does not exist in the blockchain");

            if (!IsTransactionTheLastIssueTransactionInTheVirtualChain(prevTx))
                throw new VerificationFailedException(
                    "Invalid previous transaction. Previous transaction must be the last transaction in the virtual chain.");

            //Check if this transaction and previous transaction share the main identifiers
            if (!issuerAddresses.Contains(prevTx.IssuerAddress) || !holderAddresses.Contains(prevTx.HolderAddress) ||
                !prevTx.Subject.Equals(transaction.Subject, StringComparison.OrdinalIgnoreCase) ||
                !prevTx.IdentifierKey.Equals(transaction.IdentifierKey, StringComparison.OrdinalIgnoreCase))
            {
                throw new VerificationFailedException("Transaction identifiers do not match the previous transaction's");
            }
        }
    }

    private bool IsTransactionTheLastIssueTransactionInTheVirtualChain(IdTransaction transaction)
    {
        if (transaction is not IssueIdTransaction)
            return false;

        //is this tx, the previous transaction of another tx
        var nextTransaction = _blockchainRepository.GetTransactionByPreviousTransactionHash(transaction.TransactionHash);

        return nextTransaction == null;
    }

    private List<AddressValue> GetPreviousAddressesOfTheIdCardAddress(AddressValue addressValue)
    {
        var issuerKeyChangeHistory = _blockchainRepository.GetKeyChangeHistory(addressValue);
        var issuerAddresses = new List<AddressValue> { addressValue };
        issuerAddresses.AddRange(issuerKeyChangeHistory.Select(c => c.OldAddress));
        return issuerAddresses;
    }
}