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

        /// <summary>
        /// List of transaction hashes.
        /// </summary>
        public List<string> Transactions { get; set; }

        private List<string> GetTransactions(byte[] rawData)
        {
            var span = new ReadOnlySpan<byte>(rawData);

            if (span.Length % HashValue.HashByteLength != 0)
                throw new InvalidDataException("Data length does not match the hash length.");

            var count = span.Length / HashValue.HashByteLength;
            var result = new List<string>();

            for (var i = 0; i < count; i++)
            {
                var hash = new HashValue(span.Slice(i * HashValue.HashByteLength, HashValue.HashByteLength).ToArray());
                result.Add(hash.HexString);
            }

            return result;
        }
    }
}