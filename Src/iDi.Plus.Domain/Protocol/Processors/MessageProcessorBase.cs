﻿using System.Linq;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;

namespace iDi.Plus.Domain.Protocol.Processors;

public abstract class MessageProcessorBase : IMessageProcessor
{
    protected readonly IBlockchainNodeClient BlockchainNodeClient;
    protected readonly IBlockchainRepository<IdTransaction> BlockchainRepository;
    protected readonly ILocalNodeContextProvider LocalNodeContextProvider;
    protected readonly IBlockchainNodesProvider BlockchainNodesProvider;

    protected MessageProcessorBase(IBlockchainNodeClient blockchainNodeClient, IBlockchainRepository<IdTransaction> blockchainRepository, 
        ILocalNodeContextProvider localNodeContextProvider, IBlockchainNodesProvider blockchainNodesProvider)
    {
        BlockchainNodeClient = blockchainNodeClient;
        BlockchainRepository = blockchainRepository;
        LocalNodeContextProvider = localNodeContextProvider;
        BlockchainNodesProvider = blockchainNodesProvider;
    }

    public abstract MessageTypes MessageType { get; }

    /// <inheritdoc/>
    public Message Process(Message message)
    {
        if (message.Payload.MessageType != MessageType)
            throw new InvalidInputException("Payload type does not match the target message type of this processor.");

        return ProcessPayload(message);
    }

    public bool CanProcess(Message message) => message.Header.MessageType == MessageType;

    /// <summary>
    /// Processes the incoming message
    /// </summary>
    /// <param name="message"></param>
    /// <returns>If a response should be sent back to the sender node, returns the response message, otherwise null</returns>
    public abstract Message ProcessPayload(Message message);

    /// <summary>
    /// Send message to all other nodes
    /// </summary>
    /// <param name="message">Message to send</param>
    /// <param name="excludedNodeIds">Id of the nodes to exclude when broadcasting the message.</param>
    protected void Broadcast(Message message, params string[] excludedNodeIds)
    {
        foreach (var nodeId in BlockchainNodesProvider.AllNodeIds())
        {
            if (excludedNodeIds == null || excludedNodeIds.All(e => !e.Equals(nodeId)))
                SendMessage(nodeId, message);
        }
    }

    /// <summary>
    /// Send message to a specific node
    /// </summary>
    /// <param name="nodeId">Id of the receiver node</param>
    /// <param name="message">Message to send</param>
    /// <returns>Response Message</returns>
    protected bool SendToNode(string nodeId, Message message)
    {
        return SendMessage(nodeId, message);
    }

    /// <summary>
    /// Signs a payload using local node's private key
    /// </summary>
    /// <param name="payload">Payload to sign</param>
    /// <returns>Signature</returns>
    protected byte[] SignPayload(IPayload payload)
    {
        var cryptoService = new CryptoServiceProvider();
        var signature = cryptoService.Sign(LocalNodeContextProvider.LocalKeys.PrivateKey, payload?.RawData);
        return signature;
    }

    protected bool SendMessage(string nodeId, Message messageToSend)
    {
        var node = BlockchainNodesProvider[nodeId];
        if (node != null)
            return BlockchainNodeClient.Send(node.VerifiedEndpoint1, messageToSend);

        return false;
    }
}