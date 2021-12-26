using System;
using System.Collections.Generic;
using System.Text;
using iDi.Blockchain.Framework.Protocol.iDiDirect.Exceptions;

namespace iDi.Blockchain.Framework.Protocol.iDiDirect.Payloads.MainNetwork.V1
{
    public class NewBlocksPayload : MainNetworkV1PayloadBase
    {
        public NewBlocksPayload(byte[] rawData) : base(rawData, MessageTypes.NewBlocks)
        {
            Blocks = GetBlocks(rawData);
        }

        /// <summary>
        /// List of block hashes.
        /// </summary>
        public List<string> Blocks { get; set; }

        private List<string> GetBlocks(byte[] rawData)
        {
            var span = new ReadOnlySpan<byte>(rawData);

            var txHashByteLength = Cryptography.HashAlgorithm.HashSize / 8;

            if (span.Length % txHashByteLength != 0)
                throw new InvalidDataException("Data length does not match the hash length.");
            var count = span.Length / txHashByteLength;
            var result = new List<string>();

            for (var i = 0; i < count; i++)
            {
                var hash = Encoding.ASCII.GetString(span.Slice(i * txHashByteLength, txHashByteLength));
                result.Add(hash);
            }

            return result;
        }
    }
}