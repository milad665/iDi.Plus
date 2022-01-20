using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1
{
    /// <summary>
    /// Payload of BlockData command
    /// </summary>
    public class BlockDataPayload : MainNetworkV1PayloadBase
    {
        public BlockDataPayload(byte[] rawData):base(rawData, MessageTypes.BlockData)
        {
            ExtractData(rawData);
        }

        protected BlockDataPayload(long index, HashValue hash, HashValue previousHash, DateTime timestamp, IReadOnlyCollection<TxDataPayload> transactions, long nonce, byte[] rawData) : base(rawData, MessageTypes.BlockData)
        {
            Index = index;
            Hash = hash;
            PreviousHash = previousHash;
            Timestamp = timestamp;
            Transactions = transactions;
            Nonce = nonce;
        }

        public static BlockDataPayload Create(long index, HashValue blockHash, HashValue previousHash, DateTime timestamp, IReadOnlyCollection<TxDataPayload> transactions, long nonce)
        {
            if (previousHash == null)
                previousHash = HashValue.Empty;

            if (previousHash.IsEmpty() && (index != 0 || transactions != null))
                throw new InvalidDataException("Previous transaction is empty but the block is not genesis.");

            var lstBytes = new List<byte>();
            lstBytes.AddRange(BitConverter.GetBytes(index));
            lstBytes.AddRange(blockHash.Bytes);
            lstBytes.AddRange(previousHash.Bytes);
            lstBytes.AddRange(BitConverter.GetBytes(timestamp.Ticks));
            lstBytes.AddRange(BitConverter.GetBytes(nonce));
            if (transactions is {Count: > 0})
            {
                foreach (var tx in transactions)
                {
                    lstBytes.AddRange(BitConverter.GetBytes(tx.RawData.Length));
                    lstBytes.AddRange(tx.RawData);
                }
            }
            //Last 4 Bytes must be zeros
            lstBytes.AddRange(BitConverter.GetBytes((int)0));

            return new BlockDataPayload(index, blockHash, previousHash, timestamp, transactions, nonce, lstBytes.ToArray());
        }

        public long Index { get; private set; }
        public HashValue Hash { get; private set; }
        public HashValue PreviousHash { get; private set; }
        public DateTime Timestamp { get; private set; }
        public long Nonce { get; private set; }
        public IReadOnlyCollection<TxDataPayload> Transactions { get; private set; }
        

        private void ExtractData(byte[] rawData)
        {
            var span = new ReadOnlySpan<byte>(rawData);
            var index = 0;
            Index = BitConverter.ToInt64(span.Slice(index, 8));
            index += 8;
            Hash = new HashValue(span.Slice(index, HashValue.HashByteLength).ToArray());
            index += HashValue.HashByteLength;
            PreviousHash = new HashValue(span.Slice(index, HashValue.HashByteLength).ToArray());
            index += HashValue.HashByteLength;
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