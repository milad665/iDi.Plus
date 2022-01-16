using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using iDi.Blockchain.Framework.Protocol.Extensions;

namespace iDi.Blockchain.Framework.Blockchain
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
            Hash = GetHash();
        }

        public static Block<TTransaction> Genesis()
        {
            return new Block<TTransaction>(0, "", DateTime.UtcNow, null);
        }

        public long Index { get; private set; }
        [JsonIgnore]
        public string Hash { get; private set; }
        public string PreviousHash { get; private set; }
        public DateTime Timestamp { get; private set; }
        public List<TTransaction> Transactions { get; private set; }
        public long Nonce { get; private set; }

        public bool IsGenesis() => Index == 0 && PreviousHash == "" && Transactions == null;

        public void NextNonce()
        {
            Nonce++;
            Hash = GetHash();
        }

        private string GetHash()
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(this));
            using var algorithm = FrameworkEnvironment.HashAlgorithm;
            var hashedBytes = algorithm.ComputeHash(bytes);
            return hashedBytes.ToHexString();
        }
    }
}
