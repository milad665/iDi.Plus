using System;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Exceptions;

namespace iDi.Plus.Domain.Blockchain.IdTransactions
{
    public abstract class IdTransaction : ITransaction
    {
        protected IdTransaction(string subject)
        {
            Subject = subject;
        }

        protected IdTransaction(TransactionTypes transactionType, string issuerAddress, string holderAddress, string subject, string identifierKey, string signedData, HashValue previousTransactionHash)
        {
            TransactionType = transactionType;
            IssuerAddress = issuerAddress;
            HolderAddress = holderAddress;
            IdentifierKey = identifierKey;
            SignedData = signedData;
            PreviousTransactionHash = previousTransactionHash;
            Subject = subject;
            Timestamp = DateTime.UtcNow;
        }

        public HashValue TransactionHash { get; protected set; }
        public TransactionTypes TransactionType { get; private set; }
        public string IssuerAddress { get; private set; }
        public string HolderAddress { get; private set; }
        public string Subject { get; private set; }
        public string IdentifierKey { get; private set; }
        public string SignedData { get; private set; }
        public DateTime Timestamp { get; private set; }
        public HashValue PreviousTransactionHash { get; private set; }

        /// <summary>
        /// Controls whether the transaction data is properly transmitted
        /// </summary>
        public void Verify()
        {
            if (TransactionHash != ComputeHash())
                throw new HashMismatchIdPlusException("Invalid transaction hash.");
        }

        public abstract HashValue ComputeHash();
    }
}
