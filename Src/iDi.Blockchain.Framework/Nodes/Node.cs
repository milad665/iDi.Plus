using System;
using System.Net;

namespace iDi.Blockchain.Framework.Nodes
{
    public class Node
    {
        public Guid NodeId { get; set; }
        public bool IsVerified { get; set; }
        public IPEndPoint VerifiedEndpoint1 { get; set; }
        public IPEndPoint VerifiedEndpoint2 { get; set; }
        public DateTime LastHeartbeatUtcTime { get; set; }
    }
}