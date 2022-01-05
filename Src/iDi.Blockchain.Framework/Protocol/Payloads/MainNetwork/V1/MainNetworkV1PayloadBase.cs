namespace iDi.Blockchain.Framework.Protocol.Payloads.MainNetwork.V1
{
    public abstract class MainNetworkV1PayloadBase : PayloadBase
    {
        protected MainNetworkV1PayloadBase(byte[] rawData, MessageTypes messageType) 
            : base(rawData, messageType, 1, Networks.Main)
        {
        }
    }
}
