using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Exceptions;
using iDi.Blockchain.Framework.Protocol;
using iDi.Plus.Domain.Entities;
using iDi.Plus.Node.Context;

namespace iDi.Plus.Node;

public class BlockchainNodesRepository : IBlockchainNodesRepository
{
    private readonly IdPlusDbContext _context;
    private const int MaximumNumberOfNodes = 1000;

    public BlockchainNodesRepository(IdPlusDbContext context)
    {
        _context = context;
    }

    public void AddOrUpdateNode(BlockchainNode node)
    {
        _context.Nodes.Add(Domain.Entities.Node.FromBlockchainNode(node));
        _context.SaveChanges();
    }

    public void ReplaceAllNodes(IEnumerable<BlockchainNode> nodes)
    {
        _context.Nodes.RemoveRange(_context.Nodes.ToList());
        _context.SaveChanges();

        _context.Nodes.AddRange(nodes.Select(Domain.Entities.Node.FromBlockchainNode));
        _context.SaveChanges();
    }

    public ReadOnlyDictionary<NodeIdValue, BlockchainNode> ToDictionary()
    {
        var dictionary = _context.Nodes.Cast<BlockchainNode>().ToDictionary(n => n.NodeId);
        return new ReadOnlyDictionary<NodeIdValue, BlockchainNode>(dictionary);
    }

    public IEnumerable<NodeIdValue> AllNodeIds()
    {
        return _context.Nodes.Select(n => n.NodeId).ToList();
    }

    public IEnumerable<BlockchainNode> AllNodes()
    {
        return _context.Nodes.Cast<BlockchainNode>().ToList();
    }

    public IEnumerable<BlockchainNode> GetWitnessNodes()
    {
        return _context.Nodes.Where(n => n.IsWitnessNode && n.VerifiedEndpoint1 != null).ToList();
    }

    public IEnumerable<BlockchainNode> GetBystanderNodes()
    {
        return _context.Nodes.Where(n => !n.IsWitnessNode && n.VerifiedEndpoint1 != null).OrderBy(n => Guid.NewGuid())
            .Take(MaximumNumberOfNodes).ToList();
    }

    public IEnumerable<BlockchainNode> GetDnsNodes()
    {
        return _context.Nodes.Where(n => n.IsDns && n.VerifiedEndpoint1 != null).ToList();
    }

    public BlockchainNode SelectNextWitnessNode(NodeIdValue lastNode = null)
    {
        var nodes = _context.Nodes.Where(n => n.IsWitnessNode && n.VerifiedEndpoint1 != null)
            .OrderBy(n => n.NodeId.HexString.ToLower()).ToList(); //Get ordered witness nodes

        var currentNode = CurrentWitnessTurn();

        var turn = -1;
        for (var i = 0; i < nodes.Count; i++)
        {
            if ((lastNode == null && nodes[i].NodeId.Equals(currentNode.NodeId)) || (lastNode != null && nodes[i].NodeId.Equals(lastNode)))
            {
                turn = i;
                break;
            }
        }

        if (turn == -1)
            return null;

        var currentTurn = nodes[turn];
        currentTurn.MyVote = false;

        var nextNode = nodes[(turn + 1) % nodes.Count]; //cyclic next
        nextNode.MyVote = true;

        _context.SaveChanges();

        return nodes[turn];
    }

    public BlockchainNode CurrentWitnessTurn()
    {
        var node = _context.Nodes.Where(n => n.IsWitnessNode).OrderByDescending(n => n.VotesCount).FirstOrDefault();
        return node?.VotesCount switch
        {
            0 => null,
            _ => node
        };
    }

    public void SetWitnessNodeVote(NodeIdValue sender, NodeIdValue vote)
    {
        var senderNode = _context.Nodes.FirstOrDefault(n => n.NodeId.Equals(sender));
        if (senderNode == null || !senderNode.IsWitnessNode)
            throw new VerificationFailedException("Vote sender does not exist or is not a witness node.");

        var targetNode = _context.Nodes.FirstOrDefault(n => n.NodeId.Equals(vote));
        if (targetNode == null || !targetNode.IsWitnessNode)
            return;
        
        var lastVote = _context.NodeVotes.FirstOrDefault(n => n.VoterNode.Equals(sender));
        if (lastVote != null)
        {
            var previousTargetNode = _context.Nodes.FirstOrDefault(n => n.NodeId.Equals(lastVote.Vote));
            if (previousTargetNode != null)
                previousTargetNode.VotesCount--;

            var newTargetNode = _context.Nodes.FirstOrDefault(n => n.NodeId.Equals(vote));
            if (newTargetNode != null)
                newTargetNode.VotesCount++;
            
            lastVote.Vote = vote;
        }
        else
        {
            var newTargetNode = _context.Nodes.FirstOrDefault(n => n.NodeId.Equals(vote));
            if (newTargetNode != null)
                newTargetNode.VotesCount++;

            _context.NodeVotes.Add(new NodeVote(sender, vote));
        }

        _context.SaveChanges();
    }

    public void ClearVotes()
    {
        var lastVotes = _context.NodeVotes.ToList();
        _context.NodeVotes.RemoveRange(lastVotes);

        var witnessNodes = GetWitnessNodes();
        foreach (var node in witnessNodes)
            node.VotesCount = 0;

        _context.SaveChanges();
    }

    
    public BlockchainNode GetOneRandomWitnessNode()
    {
        var node = AllNodes()
            .OrderByDescending(n => n.LastHeartbeatUtcTime)
            .FirstOrDefault(n => n.IsWitnessNode && n.VerifiedEndpoint1 != null && n.LastHeartbeatUtcTime != null);

        if (node == null)
            node = AllNodes()
                .OrderByDescending(n => n.LastHeartbeatUtcTime)
                .FirstOrDefault(n => n.IsWitnessNode && n.VerifiedEndpoint1 != null);

        if (node == null)
            throw new NotFoundException("Cannot find any witness node in the database.");

        return node;
    }

    public BlockchainNode this[string nodeId]
    {
        get
        {
            var nodeIdValue = new NodeIdValue(nodeId);
            return _context.Nodes.FirstOrDefault(n => n.NodeId.Equals(nodeIdValue));
        }
    }

    public BlockchainNode this[NodeIdValue nodeId]
    {
        get { return _context.Nodes.FirstOrDefault(n => n.NodeId.Equals(nodeId)); }
    }
}