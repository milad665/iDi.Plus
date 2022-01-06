using iDi.Blockchain.Framework.Entities;
using System;
using System.Net;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Plus.Domain.Entities
{
    public class Node : BlockchainNode, IEntity
    {
        public Node()
        {
            TimeStamp = DateTime.UtcNow;
        }

        public Node(string nodeId, bool isVerifierNode, IPEndPoint verifiedEndpoint1, IPEndPoint verifiedEndpoint2, DateTime? lastHeartbeatUtcTime, bool isDns) 
            : base(nodeId, isVerifierNode, verifiedEndpoint1, verifiedEndpoint2, lastHeartbeatUtcTime, isDns)
        {
        }

        public bool IsOriginIpAcceptable(IPAddress currentIpAddress) => !IsVerifierNode ||
                                                                        VerifiedEndpoint1.Address.Equals(
                                                                            currentIpAddress) ||
                                                                        VerifiedEndpoint2.Address.Equals(
                                                                            currentIpAddress);
        public long Id { get; }
        public DateTime TimeStamp { get; }
    }
}
