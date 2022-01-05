﻿using System;
using System.Security.Cryptography;
using iDi.Blockchain.Framework.Blockchain;

namespace iDi.Blockchain.Framework.Protocol.Payloads
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

        public byte[] RawData { get; }
        public int Checksum { get; }
        public abstract (IPayload PayloadToSend, MessageTransmissionTypes TransmissionType) Process(IBlockchainRepository blockchainRepository);

        public MessageTypes MessageType { get; }
        public short Version { get; }
        public Networks Network { get; }

        private int ComputeChecksum(byte[] rawData)
        {
            var hashAlgorithm = SHA256.Create();
            var hash= hashAlgorithm.ComputeHash(rawData);
            return BitConverter.ToInt32(hash,0);
        }
    }
}