using System;
using System.Collections.Generic;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1
{
    public class NewTxsPayload : MainNetworkV1PayloadBase
    {
        public NewTxsPayload(byte[] rawData) : base(rawData, MessageTypes.NewTxs)
        {
            Transactions = GetTransactions(rawData);
        }

        protected NewTxsPayload(byte[] rawData, List<HashValue> transactions) : base(rawData, MessageTypes.NewTxs)
        {
            Transactions = transactions;
        }

        public static NewTxsPayload Create(List<HashValue> newBlocksHashes)
        {
            var bytes = new List<byte>();
            foreach (var hash in newBlocksHashes)
                bytes.AddRange(hash.Bytes);

            return new NewTxsPayload(bytes.ToArray(), newBlocksHashes);
        }

        /// <summary>
        /// List of transaction hashes.
        /// </summary>
        public List<HashValue> Transactions { get; set; }

        private List<HashValue> GetTransactions(byte[] rawData)
        {
            var span = new ReadOnlySpan<byte>(rawData);

            if (span.Length % HashValue.HashByteLength != 0)
                throw new InvalidInputException("Data length does not match the hash length.");

            var count = span.Length / HashValue.HashByteLength;
            var result = new List<HashValue>();

            for (var i = 0; i < count; i++)
                result.Add(new HashValue(span.Slice(i * HashValue.HashByteLength, HashValue.HashByteLength).ToArray()));

            return result;
        }
    }
}