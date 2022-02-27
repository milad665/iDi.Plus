using System.Collections.Generic;
using System.Collections.ObjectModel;
using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Blockchain.Framework.Protocol;

public interface IBlockchainNodesRepository
{
    void AddOrUpdateNode(BlockchainNode node);
    void ReplaceAllNodes(IEnumerable<BlockchainNode> nodes);
    ReadOnlyDictionary<NodeIdValue, BlockchainNode> ToDictionary();
    IEnumerable<NodeIdValue> AllNodeIds();
    IEnumerable<BlockchainNode> AllNodes();

    IEnumerable<BlockchainNode> GetWitnessNodes();
    IEnumerable<BlockchainNode> GetBystanderNodes();
    IEnumerable<BlockchainNode> GetDnsNodes();

    BlockchainNode SelectNextWitnessNode(NodeIdValue lastNode = null);
    BlockchainNode CurrentWitnessTurn();
    void SetWitnessNodeVote(NodeIdValue sender, NodeIdValue vote);
    void ClearVotes();

    BlockchainNode this[string nodeId] { get; }
    BlockchainNode this[NodeIdValue nodeId] { get; }
}