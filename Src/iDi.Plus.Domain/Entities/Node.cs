using iDi.Blockchain.Framework.Entities;
using System;
using System.Net;

namespace iDi.Plus.Domain.Entities
{
    public class Node : EntityBase
    {
        public Node()
        {}

        public Node(string publicKey, bool isVerifierNode, bool isDns, IPAddress trustedIpAddress)
        {
            PublicKey = publicKey;
            IsVerifierNode = isVerifierNode;
            IsDns = isDns;
            TrustedIpAddress = trustedIpAddress;
        }

        public string PublicKey { get; private set; }
        public bool IsVerifierNode { get; private set; }
        public bool IsDns { get; private set; }
        public IPAddress TrustedIpAddress { get; private set; }
        public DateTime? LastHeartbeat { get; set; }

        public bool IsOriginIpAcceptable(IPAddress currentIpAddress) => !IsVerifierNode || TrustedIpAddress.Equals(currentIpAddress);
    }
}
