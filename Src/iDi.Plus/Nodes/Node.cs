using System;
using System.Net;

namespace iDi.Plus.Domain.Nodes
{
    public class Node
    {
        public Guid NodeId { get; set; }
        public bool IsVerified { get; set; }
        public IPAddress VerifiedEndpoint1 { get; set; }
        public IPAddress VerifiedEndpoint2 { get; set; }
        public DateTime LastHeartbeatUtcTime { get; set; }
    }
}