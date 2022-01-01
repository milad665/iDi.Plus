using iDi.Blockchain.Framework.Protocol.iDiDirect;
using iDi.Blockchain.Framework.Protocol.iDiDirect.Payloads.MainNetwork.V1;
using System;
using System.IO;

namespace iDi.Blockchain.Framework.Protocol.Payloads.MainNetwork.V1
{
    /// <summary>
    /// Payload of GetNewBlocks Request message.
    /// This is a request for all blocks added to the blockchain after the provided timestamp
    /// </summary>
    public class GetNewBlocksPayload : MainNetworkV1PayloadBase
    {
        public GetNewBlocksPayload(byte[] rawData) : base(rawData, MessageTypes.GetNewBlocks)
        {
            Timestamp = GetTimestamp(rawData);
        }

        protected GetNewBlocksPayload(DateTime timestamp, byte[] rawData) : base(rawData, MessageTypes.GetNewBlocks)
        {
            Timestamp = timestamp;
        }

        public static GetNewBlocksPayload Create(DateTime timestamp)
        {
            return new GetNewBlocksPayload(timestamp, BitConverter.GetBytes(timestamp.Ticks));
        }

        /// <summary>
        /// The timestamp after which the blocks are added to the blockchain
        /// </summary>
        public DateTime Timestamp { get; private set; }

        private DateTime GetTimestamp(byte[] rawData)
        {
            if (rawData.Length != 8)
                throw new InvalidDataException("Data length does not match length of DateTime type.");

            return DateTime.FromBinary(BitConverter.ToInt64(rawData));
        }
    }
}
