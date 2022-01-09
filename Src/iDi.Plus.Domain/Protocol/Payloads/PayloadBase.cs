using System;
using System.Security.Cryptography;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Plus.Domain.Protocol.Payloads
{
    public abstract class PayloadBase : IPayload
    {
        protected PayloadBase(byte[] rawData, MessageTypes messageType, short version, Networks network)
        {
            RawData = rawData;
            MessageType = messageType;
            Version = version;
            Network = network;

            Checksum = ComputeChecksum(rawData);
        }

        /// <inheritdoc/>
        public byte[] RawData { get; }
        /// <inheritdoc/>
        public int Checksum { get; }

        /// <inheritdoc/>
        public MessageTypes MessageType { get; }
        /// <inheritdoc/>
        public short Version { get; }
        /// <inheritdoc/>
        public Networks Network { get; }

        private int ComputeChecksum(byte[] rawData)
        {
            var hashAlgorithm = SHA256.Create();
            var hash= hashAlgorithm.ComputeHash(rawData);
            return BitConverter.ToInt32(hash,0);
        }
    }
}