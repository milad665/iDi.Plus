﻿using iDi.Blockchain.Framework.Protocol.iDiDirect;
using System.Net;

namespace iDi.Blockchain.Framework
{
    public class RequestContext
    {
        public RequestContext(Message message, EndPoint localEndpoint, EndPoint remoteEndpoint)
        {
            Message = message;
            LocalEndpoint = localEndpoint;
            RemoteEndpoint = remoteEndpoint;
        }

        public Message Message { get; }
        public EndPoint LocalEndpoint { get; }
        public EndPoint RemoteEndpoint { get; }
    }
}