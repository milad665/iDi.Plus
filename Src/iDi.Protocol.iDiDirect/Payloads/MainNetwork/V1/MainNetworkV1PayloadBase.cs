using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Messages;

namespace iDi.Protocol.iDiDirect.Payloads.MainNetwork.V1
{
    public class MainNetworkV1PayloadBase : PayloadBase
    {
        public MainNetworkV1PayloadBase(byte[] rawData, MessageTypes messageType) 
            : base(rawData, messageType, 1, Networks.Main)
        {
        }
    }
}
