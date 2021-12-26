using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace iDi.Blockchain.Framework
{
    public class Block<TTransaction> where TTransaction : ITransaction
    {
        private Block()
        {}

        public Block(long index, string previousHash, DateTime timestamp, List<TTransaction> transactions)
        {
            Index = index;
            PreviousHash = previousHash;
            Timestamp = timestamp;
            Transactions = transactions;
            Nonce = 0;
        }

        public long Index { get; private set; }
        public string Hash { get; private set; }
        public string PreviousHash { get; private set; }
        public DateTime Timestamp { get; private set; }
        public List<TTransaction> Transactions { get; private set; }
        public long Nonce { get; private set; }

        public void NextNonce()
        {
            Nonce++;
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(this));
            using var sha256Hash = SHA256.Create();
            var hashedBytes = sha256Hash.ComputeHash(bytes);
            Hash = Encoding.UTF8.GetString(hashedBytes);
        }
    }
}
