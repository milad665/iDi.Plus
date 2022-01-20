using System;
using System.Collections.Generic;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1
{
    /// <summary>
    /// Payload of CreateTx (Create Transaction) command
    /// </summary>
    public class CreateTxPayload : MainNetworkV1PayloadBase
    {
        public CreateTxPayload(byte[] rawData) : base(rawData, MessageTypes.CreateTx)
        {
            ExtractData(rawData);
        }

        protected CreateTxPayload(HashValue transactionHash, byte[] signedData, byte[] senderPublicKey, byte[] rawData) : base(rawData, MessageTypes.CreateTx)
        {
            TransactionHash = transactionHash;
            SignedData = signedData;
            SenderPublicKey = senderPublicKey;
        }

        public static CreateTxPayload Create(string transactionHash, byte[] signedData, byte[] senderPublicKey)
        {
            var lstBytes = new List<byte>();
            var hash = new HashValue(transactionHash);
            lstBytes.AddRange(hash.Bytes);
            lstBytes.AddRange(BitConverter.GetBytes(signedData.Length));
            lstBytes.AddRange(signedData);
            lstBytes.AddRange(senderPublicKey);

            return new CreateTxPayload(hash, signedData, senderPublicKey, lstBytes.ToArray());
        }

        public HashValue TransactionHash { get; private set; }
        public byte[] SignedData { get; private set; }
        public byte[] SenderPublicKey { get; private set; }


        private void ExtractData(byte[] rawData)
        {
            var span = new ReadOnlySpan<byte>(rawData);
            var index = 0;
            TransactionHash = new HashValue(span.Slice(index, HashValue.HashByteLength).ToArray());
            index += HashValue.HashByteLength;
            var signedDataLength = BitConverter.ToInt32(span.Slice(index, 4));
            index += 4;
            SignedData = span.Slice(index, signedDataLength).ToArray();
            index += signedDataLength;
            SenderPublicKey = span.Slice(index).ToArray();
        }
    }
}