using System;
using System.Collections.Generic;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Protocol.Extensions;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1
{
    public class NewBlocksPayload : MainNetworkV1PayloadBase
    {
        public NewBlocksPayload(byte[] rawData) : base(rawData, MessageTypes.NewBlocks)
        {
            Blocks = GetBlocks(rawData);
        }

        protected NewBlocksPayload(byte[] rawData, List<string> blocks) : base(rawData, MessageTypes.NewBlocks)
        {
            Blocks = blocks;
        }

        public static NewBlocksPayload Create(List<string> newBlocksHashes)
        {
            var bytes = new List<byte>();
            var txHashByteLength = FrameworkEnvironment.HashAlgorithm.HashSize / 8;

            foreach (var hash in newBlocksHashes)
            {
                var hashBytes = hash.HexStringToByteArray();
                if (hashBytes.Length != txHashByteLength)
                    throw new InvalidDataException("Block hash size does not match the hashing algorithm.");

                bytes.AddRange(hashBytes);
            }

            return new NewBlocksPayload(bytes.ToArray(), newBlocksHashes);
        }

        /// <summary>
        /// List of block hashes.
        /// </summary>
        public List<string> Blocks { get; set; }

        private List<string> GetBlocks(byte[] rawData)
        {
            var span = new ReadOnlySpan<byte>(rawData);

            var txHashByteLength = FrameworkEnvironment.HashAlgorithm.HashSize / 8;

            if (span.Length % txHashByteLength != 0)
                throw new InvalidDataException("Data length does not match the hash length.");
            var count = span.Length / txHashByteLength;
            var result = new List<string>();

            for (var i = 0; i < count; i++)
            {
                var hash = span.Slice(i * txHashByteLength, txHashByteLength).ToHexString();
                result.Add(hash);
            }

            return result;
        }
    }
}