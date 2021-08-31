using System;
using System.Security.Cryptography;
using iDi.Blockchain.Core.Messages;

namespace iDi.Protocol.iDiDirect.Payloads.MainNetwork.V1
{
    public abstract class PayloadBase : IPayload
    {
        protected PayloadBase(byte[] rawData, MessageTypes messageType)
        {
            RawData = rawData;
            MessageType = messageType;
            Checksum = ComputeChecksum(rawData);
        }

        public byte[] RawData { get; }
        public MessageTypes MessageType { get; }
        public int Checksum { get; }

        private int ComputeChecksum(byte[] rawData)
        {
            var hashAlgorithm = SHA256.Create();
            var hash= hashAlgorithm.ComputeHash(rawData);
            return BitConverter.ToInt32(hash,0);
        }
    }
}