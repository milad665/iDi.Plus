using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Extensions;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1
{
    /// <summary>
    /// Payload of get block request
    /// </summary>
    public class GetBlockPayload : MainNetworkV1PayloadBase
    {
        public GetBlockPayload(byte[] rawData) : base(rawData, MessageTypes.GetBlock)
        {
            BlockHash = rawData.ToHexString();
        }

        public GetBlockPayload(byte[] rawData, string blockHash) : base(rawData, MessageTypes.GetBlock)
        {
            BlockHash = blockHash;
        }

        public static GetBlockPayload Create(string blockHash)
        {
            var bytes = blockHash.HexStringToByteArray();
            return new GetBlockPayload(bytes, blockHash);
        }

        public string BlockHash { get; set; }
    }
}