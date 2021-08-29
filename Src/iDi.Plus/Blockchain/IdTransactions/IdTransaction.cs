using System;
using iDi.Blockchain.Core;
using iDi.Plus.Domain.Exceptions;

namespace iDi.Plus.Domain.Blockchain.IdTransactions
{
    public abstract class IdTransaction : ITransaction
    {
        protected IdTransaction()
        {
        }

        protected IdTransaction(string transactionType, string issuerPublicKey, string holderPublicKey, string identifierKey, string signedData, string previousTransactionHash)
        {
            TransactionType = transactionType;
            IssuerPublicKey = issuerPublicKey;
            HolderPublicKey = holderPublicKey;
            IdentifierKey = identifierKey;
            SignedData = signedData;
            PreviousTransactionHash = previousTransactionHash;
            Timestamp = DateTime.UtcNow;
        }

        public string TransactionHash { get; protected set; }
        public string TransactionType { get; private set; }
        public string IssuerPublicKey { get; private set; }
        public string HolderPublicKey { get; private set; }
        public string IdentifierKey { get; private set; }
        public string SignedData { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string PreviousTransactionHash { get; private set; }

        /// <summary>
        /// Controls whether the transaction data is properly transmitted
        /// </summary>
        public void Verify()
        {
            if (TransactionHash != ComputeHash())
                throw new HashMismatchDomainException("Invalid transaction hash.");
        }

        public abstract string ComputeHash();
    }
}
