﻿using System.Text;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Exceptions;

namespace iDi.Blockchain.Framework.Protocol.Payloads.MainNetwork.V1
{
    /// <summary>
    /// Payload of GetTx (Get Transaction) command
    /// </summary>
    public class GetTxPayload : MainNetworkV1PayloadBase
    {
        public GetTxPayload(byte[] rawData) : base(rawData, MessageTypes.GetTx)
        {
            TransactionHash = GetTransactionHash(rawData);
        }

        public string TransactionHash { get; set; }

        private string GetTransactionHash(byte[] rawData)
        {
            var txHashByteLength = CryptographyConstants.HashAlgorithm.HashSize / 8;

            if (rawData.Length != txHashByteLength)
                throw new InvalidDataException("Data length does not match the hash length.");

            return Encoding.ASCII.GetString(rawData);
        }
    }
}