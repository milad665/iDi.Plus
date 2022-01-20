using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Blockchain.Framework.Blockchain
{
    public class Block<TTransaction> where TTransaction : ITransaction
    {
        private Block()
        {}

        public Block(long index, HashValue previousHash, DateTime timestamp, List<TTransaction> transactions)
        {
            Index = index;
            PreviousHash = previousHash;
            Timestamp = timestamp;
            Transactions = transactions;
            Nonce = 0;
            Hash = GetHash();
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

        private HashValue GetHash()
        {
            return HashValue.ComputeHash(this);
        }
    }
}
