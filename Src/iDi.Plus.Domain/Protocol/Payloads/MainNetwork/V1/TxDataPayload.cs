using System;
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
        public const int SubjectByteLength = 256;
        public const int IdentifierByteLength = 256;

        private static readonly int TxHashByteLength = FrameworkEnvironment.HashAlgorithm.HashSize / 8;

        public TxDataPayload(byte[] rawData):base(rawData, MessageTypes.TxData)
        {
            ExtractData(rawData);
        }

        protected TxDataPayload(string transactionHash, TransactionTypes transactionType, string issuerAddress, string holderAddress, string verifierAddress, string subject, string identifierKey, DateTime timestamp, string previousTransactionHash, byte[] signedData, byte[] rawData) : base(rawData, MessageTypes.TxData)
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
            SignedData = signedData;
        }

        public static TxDataPayload Create(string transactionHash, TransactionTypes transactionType, string issuerAddress, string holderAddress, string verifierAddress, string subject, string identifierKey, DateTime timestamp, string previousTransactionHash, byte[] signedData)
        {
            if (!IdCard.IsValidAddress(issuerAddress))
                throw new InvalidDataException("Invalid IssuerAddress.");
            if (!IdCard.IsValidAddress(holderAddress))
                throw new InvalidDataException("Invalid HolderAddress.");

            if (subject.Length > SubjectByteLength)
                throw new InvalidDataException($"Subject length can not be over {SubjectByteLength} characters");

            if (identifierKey.Length > IdentifierByteLength)
                throw new InvalidDataException($"IdentifierKey length can not be over {IdentifierByteLength} characters");

            var txHashBytes = transactionHash.HexStringToByteArray();

            if (txHashBytes.Length != TxHashByteLength)
                throw new InvalidDataException($"Invalid hash length. 'TransactionHash'");

            var prevTxHashBytes = string.IsNullOrWhiteSpace(previousTransactionHash)
                ? new byte[TxHashByteLength]
                : previousTransactionHash.HexStringToByteArray();

            if (prevTxHashBytes.Length != TxHashByteLength)
                throw new InvalidDataException($"Invalid hash length. 'PreviousTransactionHash'");

            var lstBytes = new List<byte>();
            lstBytes.AddRange(txHashBytes);
            lstBytes.Add((byte)transactionType);
            lstBytes.AddRange(issuerAddress.HexStringToByteArray());
            lstBytes.AddRange(holderAddress.HexStringToByteArray());
            if (string.IsNullOrWhiteSpace(verifierAddress))
                lstBytes.AddRange(new byte[IdCard.PublicKeyByteLength]);
            else
            {
                if (!IdCard.IsValidAddress(verifierAddress))
                    throw new InvalidDataException("Invalid VerifierAddress.");

                lstBytes.AddRange(verifierAddress.HexStringToByteArray());
            }

            var subjectPadded = subject.PadRight(SubjectByteLength);
            var identifierKeyPadded = identifierKey.PadRight(IdentifierByteLength);
            lstBytes.AddRange(Encoding.ASCII.GetBytes(subjectPadded));
            lstBytes.AddRange(Encoding.ASCII.GetBytes(identifierKeyPadded));
            lstBytes.AddRange(BitConverter.GetBytes(timestamp.Ticks));
            lstBytes.AddRange(prevTxHashBytes);
            lstBytes.AddRange(signedData);

            return new TxDataPayload(transactionHash, transactionType, issuerAddress, holderAddress, verifierAddress,
                subject, identifierKey, timestamp, previousTransactionHash, signedData, lstBytes.ToArray());
        }

        public string TransactionHash { get; private set; }

        public TransactionTypes TransactionType { get; private set; }

        public string IssuingAuthorityAddress { get; private set; }

        public List<string> PrivilegeControllersAddresses { get; private set; }

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

        public string PreviousTransactionHash { get; private set; }

        public byte[] SignedData { get; private set; }

        private void ExtractData(byte[] rawData)
        {
            var span = new ReadOnlySpan<byte>(rawData);
            var index = 0;
            TransactionHash = span.Slice(index, TxHashByteLength).ToHexString();
            index += TxHashByteLength;
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
            Subject = Encoding.ASCII.GetString(span.Slice(index, SubjectByteLength)).Trim();
            index += SubjectByteLength;
            IdentifierKey = Encoding.ASCII.GetString(span.Slice(index, IdentifierByteLength)).Trim();
            index += IdentifierByteLength;
            Timestamp = DateTime.FromBinary(BitConverter.ToInt64(span.Slice(index, 8)));
            index += 8;
            var prevTxHashBytes = span.Slice(index, TxHashByteLength).ToArray();
            PreviousTransactionHash = prevTxHashBytes.All(b => b == 0) ? null : prevTxHashBytes.ToHexString();
            index += TxHashByteLength;

            SignedData = span.Slice(index).ToArray();
        }
    }
}