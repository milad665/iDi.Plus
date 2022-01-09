using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Extensions;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1
{
    /// <summary>
    /// Payload of GetTx (GetKeys Transaction) request message
    /// </summary>
    public class GetTxPayload : MainNetworkV1PayloadBase
    {
        public GetTxPayload(byte[] rawData) : base(rawData, MessageTypes.GetTx)
        {
            TransactionHash = rawData.ToHexString();
        }

        public string TransactionHash { get; set; }
    }
}