﻿using iDi.Blockchain.Framework.Entities;
using System;
using System.Net;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Plus.Domain.Entities;

public class Node : BlockchainNode, IEntity
{
    public Node()
    {
        TimeStamp = DateTime.UtcNow;
    }

    public Node(NodeIdValue nodeId, bool isWitnessNode, IPEndPoint verifiedEndpoint1, IPEndPoint verifiedEndpoint2, DateTime? lastHeartbeatUtcTime, bool isDns) 
        : base(nodeId, isWitnessNode, verifiedEndpoint1, verifiedEndpoint2, lastHeartbeatUtcTime, isDns)
    {
    }

    public bool IsOriginIpAcceptable(IPAddress currentIpAddress) => !IsWitnessNode ||
                                                                    VerifiedEndpoint1.Address.Equals(
                                                                        currentIpAddress) ||
                                                                    VerifiedEndpoint2.Address.Equals(
                                                                        currentIpAddress);
    public long Id { get; private set; }
    public DateTime TimeStamp { get; private set; }

    public static Node FromBlockchainNode(BlockchainNode node)
    {
        return new Node(node.NodeId, node.IsWitnessNode, node.VerifiedEndpoint1, node.VerifiedEndpoint2,
            node.LastHeartbeatUtcTime, node.IsDns);
    }
}