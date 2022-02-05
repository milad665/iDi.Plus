using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Blockchain.Framework.Providers;

public class BlockchainNodesProvider : IBlockchainNodesProvider
{
    private readonly Dictionary<NodeIdValue, BlockchainNode> _blockchainNodes;

    public BlockchainNodesProvider()
    {
        _blockchainNodes = new Dictionary<NodeIdValue, BlockchainNode>();
    }

    public void AddOrUpdateNode(BlockchainNode node)
    {
        if (_blockchainNodes.ContainsKey(node.NodeId))
            _blockchainNodes[node.NodeId] = node;
        else
            _blockchainNodes.Add(node.NodeId, node);
    }

    public void AddOrUpdateNodeRange(IEnumerable<BlockchainNode> nodes)
    {
        foreach (var node in nodes)
        {
            if (_blockchainNodes.ContainsKey(node.NodeId))
                _blockchainNodes[node.NodeId] = node;
            else
                _blockchainNodes.Add(node.NodeId, node);
        }
    }

    public ReadOnlyDictionary<NodeIdValue, BlockchainNode> ToDictionary()
    {
        return new ReadOnlyDictionary<NodeIdValue, BlockchainNode>(_blockchainNodes);
    }

    public IEnumerable<NodeIdValue> AllNodeIds()
    {
        return _blockchainNodes.Keys;
    }

    public IEnumerable<BlockchainNode> AllNodes()
    {
        return _blockchainNodes.Values.ToList();
    }

    public BlockchainNode this[string nodeId]
    {
        get
        {
            var nodeIdValue = new NodeIdValue(nodeId);
            if (_blockchainNodes.ContainsKey(nodeIdValue))
                return _blockchainNodes[nodeIdValue];

            return null;
        }
    }

    public BlockchainNode this[NodeIdValue nodeId]
    {
        get
        {
            if (_blockchainNodes.ContainsKey(nodeId))
                return _blockchainNodes[nodeId];

            return null;
        }
    }
}