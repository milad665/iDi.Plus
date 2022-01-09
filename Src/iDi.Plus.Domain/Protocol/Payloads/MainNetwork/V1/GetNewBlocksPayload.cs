using System;
using System.IO;
using iDi.Blockchain.Framework.Protocol;

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
            return new GetNewBlocksPayload(lastBlockIndex, BitConverter.GetBytes(lastBlockIndex));
        }

        /// <summary>
        /// Index of the last block after which the new blocks are added to the blockchain
        /// </summary>
        public long LastBlockIndex { get; private set; }

        private long GetBlockIndex(byte[] rawData)
        {
            if (rawData.Length != 8)
                throw new InvalidDataException("Data length does not match length of 'long' type.");

            return BitConverter.ToInt64(rawData);
        }
    }
}
