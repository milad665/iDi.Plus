using iDi.Blockchain.Core.Messages;

namespace iDi.Protocol.iDiDirect.Payloads.MainNetwork.V1
{
    public class CreateTxPayload : PayloadBase
    {
        public CreateTxPayload(byte[] rawData) : base(rawData, MessageTypes.CreateTx)
        {
        }
    }
}