using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Protocol.Extensions;

namespace iDi.Blockchain.Framework.Protocol.Payloads.MainNetwork.V1
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

        public string BlockHash { get; set; }
        public override (IPayload PayloadToSend, MessageTransmissionTypes TransmissionType) Process(IBlockchainRepository blockchainRepository)
        {
            throw new System.NotImplementedException();
        }
    }
}