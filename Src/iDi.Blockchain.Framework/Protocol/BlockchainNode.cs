using System;
using System.Net;
using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Blockchain.Framework.Protocol
{
    public class BlockchainNode
    {
        public BlockchainNode()
        { }

        public BlockchainNode(NodeIdValue nodeId, bool isWitnessNode, IPEndPoint verifiedEndpoint1, IPEndPoint verifiedEndpoint2, DateTime? lastHeartbeatUtcTime, bool isDns)
        {
            NodeId = nodeId;
            IsWitnessNode = isWitnessNode;
            VerifiedEndpoint1 = verifiedEndpoint1;
            VerifiedEndpoint2 = verifiedEndpoint2;
            LastHeartbeatUtcTime = lastHeartbeatUtcTime;
            IsDns = isDns;
        }

        public NodeIdValue NodeId { get; set; }
        public bool IsWitnessNode { get; set; }
        public IPEndPoint VerifiedEndpoint1 { get; set; }
        public IPEndPoint VerifiedEndpoint2 { get; set; }
        public DateTime? LastHeartbeatUtcTime { get; set; }
        public bool IsDns { get; set; }
        public int VotesCount { get; set; }
        public bool MyVote { get; set; }


        //public bool IsTurn { get; set; }
    }
}