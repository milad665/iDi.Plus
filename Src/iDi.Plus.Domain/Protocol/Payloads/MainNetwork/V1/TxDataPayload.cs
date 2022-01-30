﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Protocol.Extensions;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1
{
    public class TxDataPayload : MainNetworkV1PayloadBase
    {
        public TxDataPayload(byte[] rawData):base(rawData, MessageTypes.TxData)
        {
            ExtractData(rawData);
        }

        protected TxDataPayload(byte[] rawData, MessageTypes messageType) : base(rawData, messageType)
        {
            ExtractData(rawData);
        }

        protected TxDataPayload(HashValue transactionHash, TransactionTypes transactionType, string issuerAddress, string holderAddress, string verifierAddress, string subject, string identifierKey, DateTime timestamp, HashValue previousTransactionHash, byte[] doubleEncryptedData, byte[] rawData) : base(rawData, MessageTypes.TxData)
        {
            TransactionHash = transactionHash;
            TransactionType = transactionType;
            IssuerAddress = issuerAddress;
            HolderAddress = holderAddress;
            VerifierAddress = verifierAddress;
            Subject = subject;
            IdentifierKey = identifierKey;
            Timestamp = timestamp;
            PreviousTransactionHash = previousTransactionHash;
            DoubleEncryptedData = doubleEncryptedData;
        }

        public static TxDataPayload Create(HashValue transactionHash, TransactionTypes transactionType, string issuerAddress, string holderAddress, string verifierAddress, string subject, string identifierKey, DateTime timestamp, HashValue previousTransactionHash, byte[] signedData)
        {
            if (!IdCard.IsValidAddress(issuerAddress))
                throw new InvalidInputException("Invalid IssuerPublicKey.");
            if (!IdCard.IsValidAddress(holderAddress))
                throw new InvalidInputException("Invalid HolderPublicKey.");

            if (subject.Length > FrameworkEnvironment.SubjectByteLength)
                throw new InvalidInputException($"Subject length can not be over {FrameworkEnvironment.SubjectByteLength} characters");

            if (identifierKey.Length > FrameworkEnvironment.IdentifierByteLength)
                throw new InvalidInputException($"IdentifierKey length can not be over {FrameworkEnvironment.IdentifierByteLength} characters");

            if (previousTransactionHash == null)
                previousTransactionHash = HashValue.Empty;

            var lstBytes = new List<byte>();
            lstBytes.AddRange(transactionHash.Bytes);
            lstBytes.Add((byte)transactionType);
            lstBytes.AddRange(issuerAddress.HexStringToByteArray());
            lstBytes.AddRange(holderAddress.HexStringToByteArray());
            if (string.IsNullOrWhiteSpace(verifierAddress))
                lstBytes.AddRange(new byte[IdCard.PublicKeyByteLength]);
            else
            {
                if (!IdCard.IsValidAddress(verifierAddress))
                    throw new InvalidInputException("Invalid VerifierAddress.");

                lstBytes.AddRange(verifierAddress.HexStringToByteArray());
            }

            var subjectPadded = subject.PadRight(FrameworkEnvironment.SubjectByteLength);
            var identifierKeyPadded = identifierKey.PadRight(FrameworkEnvironment.IdentifierByteLength);
            lstBytes.AddRange(Encoding.ASCII.GetBytes(subjectPadded));
            lstBytes.AddRange(Encoding.ASCII.GetBytes(identifierKeyPadded));
            lstBytes.AddRange(BitConverter.GetBytes(timestamp.Ticks));
            lstBytes.AddRange(previousTransactionHash.Bytes);
            lstBytes.AddRange(signedData);

            return new TxDataPayload(transactionHash, transactionType, issuerAddress, holderAddress, verifierAddress,
                subject, identifierKey, timestamp, previousTransactionHash, signedData, lstBytes.ToArray());
        }

        public HashValue TransactionHash { get; private set; }

        public TransactionTypes TransactionType { get; private set; }

        /// <summary>
        /// Reserved - Initial Issuer
        /// </summary>
        public string IssuingAuthorityAddress { get; private set; }
        /// <summary>
        /// Reserved - Controllers with privilege to change access writes
        /// </summary>
        public List<string> PrivilegeControllersAddresses { get; private set; }
        /// <summary>
        /// Reserved - Controllers
        /// </summary>
        public List<string> ControllersAddresses { get; private set; }

        public string IssuerAddress { get; private set; }

        public string HolderAddress { get; private set; }

        /// <summary>
        /// This property is only filled for consent transactions. The value will be null for issue transactions.
        /// </summary>
        public string VerifierAddress { get; private set; }

        public string Subject { get; private set; }

        public string IdentifierKey { get; private set; }

        public DateTime Timestamp { get; private set; }

        public HashValue PreviousTransactionHash { get; private set; }

        public byte[] DoubleEncryptedData { get; private set; }

        private void ExtractData(byte[] rawData)
        {
            var span = new ReadOnlySpan<byte>(rawData);
            var index = 0;
            TransactionHash = new HashValue(span.Slice(index, HashValue.HashByteLength).ToArray());
            index += HashValue.HashByteLength;
            TransactionType = (TransactionTypes) span.Slice(index, 1)[0];
            index++;
            IssuerAddress = span.Slice(index, IdCard.PublicKeyByteLength).ToHexString();
            index += IdCard.PublicKeyByteLength;
            HolderAddress = span.Slice(index, IdCard.PublicKeyByteLength).ToHexString();
            index += IdCard.PublicKeyByteLength;
            var verifierAddressBytes = span.Slice(index, IdCard.PublicKeyByteLength).ToArray();
            if (verifierAddressBytes.Any(b => b != 0)) //Verifier address memory-space contains a value
                VerifierAddress = verifierAddressBytes.ToHexString();
            
            index += IdCard.PublicKeyByteLength;
            Subject = Encoding.ASCII.GetString(span.Slice(index, FrameworkEnvironment.SubjectByteLength)).Trim();
            index += FrameworkEnvironment.SubjectByteLength;
            IdentifierKey = Encoding.ASCII.GetString(span.Slice(index, FrameworkEnvironment.IdentifierByteLength)).Trim();
            index += FrameworkEnvironment.IdentifierByteLength;
            Timestamp = DateTime.FromBinary(BitConverter.ToInt64(span.Slice(index, 8)));
            index += 8;
            PreviousTransactionHash = new HashValue(span.Slice(index, HashValue.HashByteLength).ToArray());
            index += HashValue.HashByteLength;

            DoubleEncryptedData = span.Slice(index).ToArray();
        }
    }
}