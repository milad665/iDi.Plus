using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Exceptions;

namespace iDi.Blockchain.Framework.Blockchain
{
    public class Block<TTransaction> where TTransaction : ITransaction
    {
        private Block()
        {}

        public Block(long index, HashValue hash, HashValue previousHash, DateTime timestamp, List<TTransaction> transactions, long nonce)
        {
            Index = index;
            Hash = hash;
            PreviousHash = previousHash;
            Timestamp = timestamp;
            Transactions = transactions;
            Nonce = nonce;
        }

        private Block(long index, HashValue previousHash, DateTime timestamp, List<TTransaction> transactions)
        {
            Index = index;
            PreviousHash = previousHash;
            Timestamp = timestamp;
            Transactions = transactions;
            Nonce = 0;
            Hash = GetHash();
        }

        public static Block<TTransaction> Create(long index, HashValue previousHash, DateTime timestamp, List<TTransaction> transactions)
        {
            return new Block<TTransaction>(index, previousHash, timestamp, transactions);
        }

        public static Block<TTransaction> Genesis()
        {
            return new Block<TTransaction>(0, HashValue.Empty, DateTime.UtcNow, null);
        }

        public long Index { get; private set; }
        [JsonIgnore]
        public HashValue Hash { get; private set; }
        public HashValue PreviousHash { get; private set; }
        public DateTime Timestamp { get; private set; }
        public List<TTransaction> Transactions { get; private set; }
        public long Nonce { get; private set; }

        public bool IsGenesis() => Index == 0 && PreviousHash.IsEmpty() && Transactions == null;

        public void NextNonce()
        {
            Nonce++;
            Hash = GetHash();
        }

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
}
