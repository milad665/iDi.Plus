using System;
using System.Collections.Generic;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1
{
    public class NewBlocksPayload : MainNetworkV1PayloadBase
    {
        public NewBlocksPayload(byte[] rawData) : base(rawData, MessageTypes.NewBlocks)
        {
            Blocks = GetBlocks(rawData);
        }

        protected NewBlocksPayload(byte[] rawData, List<HashValue> blocks) : base(rawData, MessageTypes.NewBlocks)
        {
            Blocks = blocks;
        }

        public static NewBlocksPayload Create(List<HashValue> newBlocksHashes)
        {
            var bytes = new List<byte>();
            foreach (var hash in newBlocksHashes)
                bytes.AddRange(hash.Bytes);

            return new NewBlocksPayload(bytes.ToArray(), newBlocksHashes);
        }

        /// <summary>
        /// List of block hashes.
        /// </summary>
        public List<HashValue> Blocks { get; set; }

        private List<HashValue> GetBlocks(byte[] rawData)
        {
            var span = new ReadOnlySpan<byte>(rawData);

            var txHashByteLength = HashValue.HashByteLength;

            if (span.Length % txHashByteLength != 0)
                throw new InvalidInputException("Data length does not match the hash length.");
            var count = span.Length / txHashByteLength;
            var result = new List<HashValue>();

            for (var i = 0; i < count; i++)
                result.Add(new HashValue(span.Slice(i * txHashByteLength, txHashByteLength).ToArray()));

            return result;
        }
    }
}