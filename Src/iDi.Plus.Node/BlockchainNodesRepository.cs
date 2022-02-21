﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Plus.Node.Context;

namespace iDi.Plus.Node;

public class BlockchainNodesRepository : IBlockchainNodesRepository
{
    private Dictionary<NodeIdValue, BlockchainNode> _blockchainNodesCache;
    private readonly IdPlusDbContext _context;

    public BlockchainNodesRepository(IdPlusDbContext context)
    {
        _context = context;
        _blockchainNodesCache = new Dictionary<NodeIdValue, BlockchainNode>();

        RefreshCache();
    }

    public void AddOrUpdateNode(BlockchainNode node)
    {
        _context.Nodes.Add(Domain.Entities.Node.FromBlockchainNode(node));
        _context.SaveChanges();

        RefreshCache();
    }

    public void ReplaceAllNodes(IEnumerable<BlockchainNode> nodes)
    {
        _context.Nodes.RemoveRange(_context.Nodes.ToList());
        _context.SaveChanges();

        _context.Nodes.AddRange(nodes.Select(Domain.Entities.Node.FromBlockchainNode));
        _context.SaveChanges();

        RefreshCache();
    }

    public ReadOnlyDictionary<NodeIdValue, BlockchainNode> ToDictionary()
    {
        return new ReadOnlyDictionary<NodeIdValue, BlockchainNode>(_blockchainNodesCache);
    }

    public IEnumerable<NodeIdValue> AllNodeIds()
    {
        return _blockchainNodesCache.Keys;
    }

    public IEnumerable<BlockchainNode> AllNodes()
    {
        return _blockchainNodesCache.Values.ToList();
    }

    public IEnumerable<BlockchainNode> GetWitnessNodes()
    {
        return _context.Nodes.Where(n => n.IsWitnessNode && n.VerifiedEndpoint1 != null).ToList();
    }

    public IEnumerable<BlockchainNode> GetDnsNodes()
    {
        return _context.Nodes.Where(n => n.IsDns && n.VerifiedEndpoint1 != null).ToList();
    }

    public BlockchainNode SelectNextWitnessNode(NodeIdValue lastNode = null)
    {
        var nodes = _context.Nodes.Where(n => n.IsWitnessNode && n.VerifiedEndpoint1 != null)
            .OrderBy(n => n.NodeId.HexString.ToLower()).ToList(); //Get ordered witness nodes

        var turn = -1;
        for (var i = 0; i < nodes.Count; i++)
        {
            if ((lastNode == null && nodes[i].IsTurn) || (lastNode != null && nodes[i].NodeId.Equals(lastNode)))
            {
                turn = i;
                break;
            }
        }

        if (turn == -1)
            return null;

        var currentNode = nodes[turn];
        currentNode.IsTurn = false;
        _context.Nodes.Update(currentNode);

        var nextNode = nodes[turn == nodes.Count - 1 ? 0 : turn + 1]; //cyclic next
        nextNode.IsTurn = true;
        _context.Nodes.Update(nextNode);

        _context.SaveChanges();
        RefreshCache();

        return nodes[turn];
    }

    public BlockchainNode this[string nodeId]
    {
        get
        {
            var nodeIdValue = new NodeIdValue(nodeId);
            if (_blockchainNodesCache.ContainsKey(nodeIdValue))
                return _blockchainNodesCache[nodeIdValue];

            return null;
        }
    }

    public BlockchainNode this[NodeIdValue nodeId]
    {
        get
        {
            if (_blockchainNodesCache.ContainsKey(nodeId))
                return _blockchainNodesCache[nodeId];

            return null;
        }
    }

    private void RefreshCache()
    {
        var total = 1000;
        var witnesses = _context.Nodes.Where(n => n.IsWitnessNode).ToList();
        var nonWitnesses = _context.Nodes.Where(n => !n.IsWitnessNode).Take(total - witnesses.Count).ToList();
        _blockchainNodesCache = witnesses.Union(nonWitnesses).Cast<BlockchainNode>().ToList().ToDictionary(n => n.NodeId);
    }
}