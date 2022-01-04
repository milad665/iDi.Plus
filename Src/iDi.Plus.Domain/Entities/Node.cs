using iDi.Blockchain.Framework.Entities;
using System;
using System.Net;

namespace iDi.Plus.Domain.Entities
{
    public class Node : EntityBase
    {
        public Node()
        {}

        public Node(string publicKey, bool isVerifierNode, bool isDns, IPEndPoint ipEndpoint)
        {
            PublicKey = publicKey;
            IsVerifierNode = isVerifierNode;
            IsDns = isDns;
            IpEndpoint = ipEndpoint;
        }

        public string PublicKey { get; private set; }
        public bool IsVerifierNode { get; private set; }
        public bool IsDns { get; private set; }
        public IPEndPoint IpEndpoint { get; private set; }
        public DateTime? LastHeartbeat { get; set; }

        public bool IsOriginIpAcceptable(IPAddress currentIpAddress) => !IsVerifierNode || IpEndpoint.Address.Equals(currentIpAddress);
    }
}
