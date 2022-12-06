using System;
using System.Linq;
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

        protected IdTransaction(TransactionTypes transactionType, AddressValue issuerAddress, AddressValue holderAddress, string subject, string identifierKey, string valueMimeType, byte[] doubleEncryptedData, HashValue previousTransactionHash)
        {
            TransactionType = transactionType;
            IssuerAddress = issuerAddress;
            HolderAddress = holderAddress;
            IdentifierKey = identifierKey;
            PreviousTransactionHash = previousTransactionHash;
            ValueMimeType = valueMimeType;
            DoubleEncryptedData = doubleEncryptedData;
            Subject = subject;
            Timestamp = DateTime.UtcNow;
        }

        public HashValue TransactionHash { get; protected set; }
        public TransactionTypes TransactionType { get; private set; }
        public AddressValue IssuerAddress { get; private set; }
        public AddressValue HolderAddress { get; private set; }
        public string Subject { get; private set; }
        public string IdentifierKey { get; private set; }
        public string ValueMimeType { get; private set; }
        public byte[] DoubleEncryptedData { get; private set; }
        public DateTime Timestamp { get; private set; }
        public HashValue PreviousTransactionHash { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            if (GetType().GetProperties().Any(propertyInfo => propertyInfo.GetValue(this) != propertyInfo.GetValue(obj)))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(TransactionHash);
            hashCode.Add((int)TransactionType);
            hashCode.Add(IssuerAddress);
            hashCode.Add(HolderAddress);
            hashCode.Add(Subject);
            hashCode.Add(IdentifierKey);
            hashCode.Add(ValueMimeType);
            hashCode.Add(DoubleEncryptedData);
            hashCode.Add(Timestamp);
            hashCode.Add(PreviousTransactionHash);
            return hashCode.ToHashCode();
        }
    }
}
