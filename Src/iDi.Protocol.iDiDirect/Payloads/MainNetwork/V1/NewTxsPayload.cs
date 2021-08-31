using System;
using System.Collections.Generic;
using System.Text;
using iDi.Blockchain.Core;
using iDi.Blockchain.Core.Messages;
using iDi.Protocol.iDiDirect.Exceptions;

namespace iDi.Protocol.iDiDirect.Payloads.MainNetwork.V1
{
    public class NewTxsPayload : PayloadBase
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