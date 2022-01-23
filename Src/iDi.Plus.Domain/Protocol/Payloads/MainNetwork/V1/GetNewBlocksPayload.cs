using System;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1
{
    /// <summary>
    /// Payload of GetNewBlocks Request message.
    /// This is a request for all blocks added to the blockchain after the provided timestamp
    /// </summary>
    public class GetNewBlocksPayload : MainNetworkV1PayloadBase
    {
        public GetNewBlocksPayload(byte[] rawData) : base(rawData, MessageTypes.GetNewBlocks)
        {
            LastBlockIndex = GetBlockIndex(rawData);
        }

        protected GetNewBlocksPayload(long lastBlockIndex, byte[] rawData) : base(rawData, MessageTypes.GetNewBlocks)
        {
            LastBlockIndex = lastBlockIndex;
        }

        public static GetNewBlocksPayload Create(long lastBlockIndex)
        {
            if (lastBlockIndex < 0)
                throw new InvalidInputException("Block index cannot be negative.");

            return new GetNewBlocksPayload(lastBlockIndex, BitConverter.GetBytes(lastBlockIndex));
        }

        /// <summary>
        /// Index of the last block after which the new blocks are added to the blockchain
        /// </summary>
        public long LastBlockIndex { get; private set; }

        private long GetBlockIndex(byte[] rawData)
        {
            if (rawData.Length != 8)
                throw new InvalidInputException("Data length does not match length of 'long' type.");

            var lastBlockIndex = BitConverter.ToInt64(rawData);
            if (lastBlockIndex < 0)
                throw new InvalidInputException("Block index cannot be negative.");

            return lastBlockIndex;
        }
    }
}
