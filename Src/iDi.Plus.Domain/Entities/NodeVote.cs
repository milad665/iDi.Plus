using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Plus.Domain.Entities;

public class NodeVote
{
    public NodeVote()
    {}

    public NodeVote(NodeIdValue voterNode, NodeIdValue vote)
    {
        VoterNode = voterNode;
        Vote = vote;
    }

    public NodeIdValue VoterNode { get; set; }
    public NodeIdValue Vote { get; set; }
}