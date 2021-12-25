using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iDi.Blockchain.Core;
using iDi.Blockchain.Core.Messages;
using iDi.Protocol.iDiDirect.Exceptions;
using iDi.Protocol.iDiDirect.Extensions;

namespace iDi.Protocol.iDiDirect.Payloads.MainNetwork.V1
{
    public class TxDataPayload : PayloadBase
    {
        private const int SubjectByteLength = 256;
        private const int IdentifierByteLength = 256;

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
            if (IsValidAddress(issuerAddress))
                throw new InvalidDataException("Invalid IssuerAddress.");
            if (IsValidAddress(holderAddress))
                throw new InvalidDataException("Invalid HolderAddress.");

            if (subject.Length > SubjectByteLength / 2)
                throw new InvalidDataException($"Subject length can not be over {SubjectByteLength / 2} unicode characters");

            if (identifierKey.Length > IdentifierByteLength / 2)
                throw new InvalidDataException($"IdentifierKey length can not be over {IdentifierByteLength / 2} unicode characters");

            var lstBytes = new List<byte>();
            lstBytes.AddRange(transactionHash.HexStringToByteArray());
            lstBytes.AddRange(BitConverter.GetBytes((byte)transactionType));
            lstBytes.AddRange(issuerAddress.HexStringToByteArray());
            lstBytes.AddRange(holderAddress.HexStringToByteArray());
            if (string.IsNullOrWhiteSpace(verifierAddress))
                lstBytes.AddRange(new byte[Cryptography.WalletAddressByteLengthExcludingPrefix]);
            else
            {
                if (IsValidAddress(verifierAddress))
                    throw new InvalidDataException("Invalid VerifierAddress.");

                lstBytes.AddRange(verifierAddress.HexStringToByteArray());
            }

            var subjectPadded = subject.PadRight(SubjectByteLength / 2);
            var identifierKeyPadded = identifierKey.PadRight(IdentifierByteLength / 2);
            lstBytes.AddRange(Encoding.Unicode.GetBytes(subjectPadded));
            lstBytes.AddRange(Encoding.Unicode.GetBytes(identifierKeyPadded));
            lstBytes.AddRange(BitConverter.GetBytes(timestamp.Ticks));
            lstBytes.AddRange(previousTransactionHash.HexStringToByteArray());
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
            var txHashByteLength = Cryptography.HashAlgorithm.HashSize / 8;

            var span = new ReadOnlySpan<byte>(rawData);
            var index = 0;
            TransactionHash = span.Slice(index, txHashByteLength).ToHexString();
            index += txHashByteLength;
            TransactionType = (TransactionTypes) span.Slice(index, 1)[0];
            index++;
            IssuerAddress = $"{Cryptography.WalletAddressPrefix}{span.Slice(index, Cryptography.WalletAddressByteLengthExcludingPrefix).ToHexString()}";
            index += Cryptography.WalletAddressByteLengthExcludingPrefix;
            HolderAddress = $"{Cryptography.WalletAddressPrefix}{span.Slice(index, Cryptography.WalletAddressByteLengthExcludingPrefix).ToHexString()}";
            index += Cryptography.WalletAddressByteLengthExcludingPrefix;
            var verifierAddressBytes = span.Slice(index, Cryptography.WalletAddressByteLengthExcludingPrefix).ToArray();
            if (verifierAddressBytes.Any(b => b != 0)) //Verifier address memory-space contains a value
                VerifierAddress = $"{Cryptography.WalletAddressPrefix}{verifierAddressBytes.ToHexString()}";
            
            index += Cryptography.WalletAddressByteLengthExcludingPrefix;
            Subject = Encoding.Unicode.GetString(span.Slice(index, SubjectByteLength)).Trim();
            index += SubjectByteLength;
            IdentifierKey = Encoding.Unicode.GetString(span.Slice(index, IdentifierByteLength)).Trim();
            index += IdentifierByteLength;
            Timestamp = DateTime.FromBinary(BitConverter.ToInt64(span.Slice(index, 8)));
            index += 8;
            PreviousTransactionHash = span.Slice(index, txHashByteLength).ToHexString();
            index += txHashByteLength;

            SignedData = span.Slice(index).ToArray();
        }

        private static bool IsValidAddress(string walletAddress)
        {
            if (string.IsNullOrWhiteSpace(walletAddress) || !walletAddress.ToUpper().StartsWith(Cryptography.WalletAddressPrefix.ToUpper()))
                return false;

            walletAddress = walletAddress.Substring(3);

            return walletAddress.Length == 2 * Cryptography.WalletAddressByteLengthExcludingPrefix;
        }
    }
}