using iDi.Blockchain.Core;
using iDi.Blockchain.Core.Messages;

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
