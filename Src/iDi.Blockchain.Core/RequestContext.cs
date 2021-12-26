using iDi.Blockchain.Core.Messages;
using System.Net;

namespace iDi.Blockchain.Core
{
    public class RequestContext
    {
        public RequestContext(IMessage message, EndPoint localEndpoint, EndPoint remoteEndpoint)
        {
            Message = message;
            LocalEndpoint = localEndpoint;
            RemoteEndpoint = remoteEndpoint;
        }

        public IMessage Message { get; }
        public EndPoint LocalEndpoint { get; }
        public EndPoint RemoteEndpoint { get; }
    }
}
