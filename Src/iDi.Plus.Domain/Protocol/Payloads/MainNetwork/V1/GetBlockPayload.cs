using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1
{
    /// <summary>
    /// Payload of get block request
    /// </summary>
    public class GetBlockPayload : MainNetworkV1PayloadBase
    {
        public GetBlockPayload(byte[] rawData) : base(rawData, MessageTypes.GetBlock)
        {
            BlockHash = new HashValue(rawData);
        }

        protected GetBlockPayload(byte[] rawData, HashValue blockHash) : base(rawData, MessageTypes.GetBlock)
        {
            BlockHash = blockHash;
        }

        public static GetBlockPayload Create(HashValue blockHash)
        {
            return new GetBlockPayload(blockHash.Bytes, blockHash);
        }

        public HashValue BlockHash { get; set; }
    }
}