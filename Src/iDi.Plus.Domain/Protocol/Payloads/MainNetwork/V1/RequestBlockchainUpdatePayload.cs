using System;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1
{
    /// <summary>
    /// Payload of GetNewBlocks Request message.
    /// This is a request for all blocks added to the blockchain after the provided timestamp
    /// </summary>
    public class RequestBlockchainUpdatePayload : MainNetworkV1PayloadBase
    {
        public RequestBlockchainUpdatePayload(byte[] rawData) : base(rawData, MessageTypes.RequestBlockchainUpdate)
        {
            LastBlockIndex = GetBlockIndex(rawData);
        }

        private RequestBlockchainUpdatePayload(long lastBlockIndex, byte[] rawData) : base(rawData, MessageTypes.RequestBlockchainUpdate)
        {
            LastBlockIndex = lastBlockIndex;
        }

        public static RequestBlockchainUpdatePayload Create(long lastBlockIndex)
        {
            if (lastBlockIndex < 0)
                throw new InvalidInputException("Block index cannot be negative.");

            return new RequestBlockchainUpdatePayload(lastBlockIndex, BitConverter.GetBytes(lastBlockIndex));
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
