using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using iDi.Blockchain.Core;
using iDi.Blockchain.Core.Messages;
using iDi.Protocol.iDiDirect.Extensions;

namespace iDi.Protocol.iDiDirect.Payloads.MainNetwork.V1
{
    public class BlockDataPayload : PayloadBase
    {
        public BlockDataPayload(byte[] rawData):base(rawData, MessageTypes.BlockData)
        {
            ExtractData(rawData);
        }

        protected BlockDataPayload(long index, string hash, string previousHash, DateTime timestamp, IReadOnlyCollection<TxDataPayload> transactions, long nonce, byte[] rawData) : base(rawData, MessageTypes.BlockData)
        {
            Index = index;
            Hash = hash;
            PreviousHash = previousHash;
            Timestamp = timestamp;
            Transactions = transactions;
            Nonce = nonce;
        }

        public static BlockDataPayload Create(long index, string hash, string previousHash, DateTime timestamp, IReadOnlyCollection<TxDataPayload> transactions, long nonce)
        {
            var lstBytes = new List<byte>();
            lstBytes.AddRange(BitConverter.GetBytes(index));
            lstBytes.AddRange(hash.HexStringToByteArray());
            lstBytes.AddRange(previousHash.HexStringToByteArray());
            lstBytes.AddRange(BitConverter.GetBytes(timestamp.Ticks));
            lstBytes.AddRange(BitConverter.GetBytes(nonce));
            foreach (var tx in transactions)
            {
                lstBytes.AddRange(BitConverter.GetBytes(tx.RawData.Length));
                lstBytes.AddRange(tx.RawData);
            }

            //Last 4 Bytes must be zeros
            lstBytes.AddRange(BitConverter.GetBytes((int)0));

            return new BlockDataPayload(index, hash, previousHash, timestamp, transactions, nonce, lstBytes.ToArray());
        }

        public long Index { get; private set; }
        public string Hash { get; private set; }
        public string PreviousHash { get; private set; }
        public DateTime Timestamp { get; private set; }
        public IReadOnlyCollection<TxDataPayload> Transactions { get; private set; }
        public long Nonce { get; private set; }

        private void ExtractData(byte[] rawData)
        {
            var txHashByteLength = Cryptography.HashAlgorithm.HashSize / 8;

            var span = new ReadOnlySpan<byte>(rawData);
            var index = 0;
            Index = BitConverter.ToInt64(span.Slice(index, 8));
            index += 8;
            Hash = span.Slice(index, txHashByteLength).ToHexString();
            index += txHashByteLength;
            PreviousHash = span.Slice(index, txHashByteLength).ToHexString();
            index += txHashByteLength;
            Timestamp = DateTime.FromBinary(BitConverter.ToInt64(span.Slice(index, 8)));
            index += 8;
            Nonce = BitConverter.ToInt64(span.Slice(index, 8));
            index += 8;

            var transactionDataByteLength = BitConverter.ToInt32(span.Slice(index, 4));
            index += 4;
            
            var lstTransactions = new List<TxDataPayload>();

            while (transactionDataByteLength > 0)
            {
                var transactionBytes = span.Slice(index, transactionDataByteLength);
                index += transactionDataByteLength;
                var txData = new TxDataPayload(transactionBytes.ToArray());
                lstTransactions.Add(txData);
                transactionDataByteLength = BitConverter.ToInt32(span.Slice(index, 4));
                index += 4;
            }

            Transactions = new ReadOnlyCollection<TxDataPayload>(lstTransactions);
        }
    }
}