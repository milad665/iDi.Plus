﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Plus.Domain.Blockchain.IdTransactions;

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

        private BlockDataPayload(long index, HashValue hash, HashValue previousHash, DateTime timestamp, IReadOnlyCollection<TxDataResponsePayload> transactions, long nonce, byte[] rawData) : base(rawData, MessageTypes.BlockData)
        {
            Index = index;
            Hash = hash;
            PreviousHash = previousHash;
            Timestamp = timestamp;
            Transactions = transactions;
            Nonce = nonce;
        }

        public static BlockDataPayload Create(long index, HashValue blockHash, HashValue previousHash, DateTime timestamp, IReadOnlyCollection<TxDataResponsePayload> transactions, long nonce)
        {
            if (previousHash == null)
                previousHash = HashValue.Empty;

            if (previousHash.IsEmpty() && (index != 0 || transactions != null))
                throw new InvalidInputException("Previous transaction is empty but the block is not genesis.");

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

        public static BlockDataPayload FromBlock(Block<IdTransaction> block) => Create(block.Index, block.Hash,
            block.PreviousHash, block.Timestamp,
            block.Transactions.Select(TxDataResponsePayload.FromIdTransaction).ToList(), block.Nonce);

        public long Index { get; private set; }
        public HashValue Hash { get; private set; }
        public HashValue PreviousHash { get; private set; }
        public DateTime Timestamp { get; private set; }
        public long Nonce { get; private set; }
        public IReadOnlyCollection<TxDataResponsePayload> Transactions { get; private set; }

        public Block<IdTransaction> ToBlock(IIdTransactionFactory idTransactionFactory)
        {
            var transactions = Transactions.Select(idTransactionFactory.CreateFromTxDataResponsePayload).ToList();

            return new Block<IdTransaction>(Index, Hash, PreviousHash, Timestamp, transactions, Nonce);
        }

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
            
            var lstTransactions = new List<TxDataResponsePayload>();

            while (transactionDataByteLength > 0)
            {
                var transactionBytes = span.Slice(index, transactionDataByteLength);
                index += transactionDataByteLength;
                var txData = new TxDataResponsePayload(transactionBytes.ToArray());
                lstTransactions.Add(txData);
                transactionDataByteLength = BitConverter.ToInt32(span.Slice(index, 4));
                index += 4;
            }

            Transactions = new ReadOnlyCollection<TxDataResponsePayload>(lstTransactions);
        }
    }
}