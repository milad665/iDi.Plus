using System.Net;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Extensions;

namespace iDi.Blockchain.Framework
{
    public class RequestContext
    {
        public RequestContext(Message message, EndPoint localEndpoint, EndPoint remoteEndpoint, KeyPair localKeys)
        {
            Message = message;
            LocalEndpoint = localEndpoint;
            RemoteEndpoint = remoteEndpoint;
            LocalKeys = localKeys;

            LocalNodeId = localKeys.PublicKey.ToHexString();
        }

        public Message Message { get; }
        public EndPoint LocalEndpoint { get; }
        public EndPoint RemoteEndpoint { get; }
        public KeyPair LocalKeys { get; }
        public string LocalNodeId { get; }
    }
}
