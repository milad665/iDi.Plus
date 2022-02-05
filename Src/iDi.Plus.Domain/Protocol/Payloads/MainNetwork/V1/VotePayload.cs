using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

public class VotePayload : MainNetworkV1PayloadBase
{
    public VotePayload(byte[] rawData) : base(rawData, MessageTypes.Vote)
    {
        NodeId = new NodeIdValue(rawData);
    }

    protected VotePayload(byte[] rawData, NodeIdValue nodeId) : base(rawData, MessageTypes.Vote)
    {
        NodeId = nodeId;
    }

    public static VotePayload Create(NodeIdValue nodeId)
    {
        return new VotePayload(nodeId.Bytes, nodeId);
    }

    public NodeIdValue NodeId { get; set; }
}