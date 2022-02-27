using System;
using System.Text.Json.Serialization;
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

        protected IdTransaction(TransactionTypes transactionType, AddressValue issuerAddress, AddressValue holderAddress, string subject, string identifierKey, byte[] signedData, HashValue previousTransactionHash)
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

        [JsonIgnore]
        public HashValue TransactionHash { get; protected set; }
        public TransactionTypes TransactionType { get; private set; }
        public AddressValue IssuerAddress { get; private set; }
        public AddressValue HolderAddress { get; private set; }
        public string Subject { get; private set; }
        public string IdentifierKey { get; private set; }
        public byte[] SignedData { get; private set; }
        public DateTime Timestamp { get; private set; }
        public HashValue PreviousTransactionHash { get; private set; }

        /// <summary>
        /// Controls whether the transaction data is properly transmitted
        /// </summary>
        public void VerifyHash()
        {
            if (TransactionHash != HashValue.ComputeHash(this))
                throw new VerificationFailedException("Invalid transaction hash.");
        }
    }
}
