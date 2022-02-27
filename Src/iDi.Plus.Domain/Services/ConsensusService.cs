using System;
using System.Linq;
using System.Net;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Services;

public class ConsensusService : IConsensusService
{

    private readonly IBlockchain<IdTransaction> _blockchain;
    private readonly IHotPoolRepository<IdTransaction> _hotPoolRepository;
    private readonly IBlockchainNodesRepository _blockchainNodesRepository;
    private readonly IBlockchainNodeClient _blockchainNodeClient;
    private readonly ILocalNodeContextProvider _localNodeContextProvider;
    private readonly IGlobalVariablesRepository _globalVariablesRepository;

    private const string HotPoolSizeWhenNodeWasSelectedGlobalVariableKey = "HotPoolSizeWhenNodeWasSelected";

    public const int MaximumNumberOfTransactionsInABlock = 1000;
    public const int HotPoolSizeThreshold = 100;
    public const int MinutesToWaitBeforeCreatingNewBlock = 5;

    public event Action<ConsensusService, BlockCreatedEventArgs> BlockCreated;

    public ConsensusService(IHotPoolRepository<IdTransaction> hotPoolRepository, 
        IBlockchain<IdTransaction> blockchain,
        IBlockchainNodesRepository blockchainNodesRepository, 
        IBlockchainNodeClient blockchainNodeClient, 
        ILocalNodeContextProvider localNodeContextProvider, 
        IGlobalVariablesRepository globalVariablesRepository)
    {
        _hotPoolRepository = hotPoolRepository;
        _blockchain = blockchain;
        _blockchainNodesRepository = blockchainNodesRepository;
        _blockchainNodeClient = blockchainNodeClient;
        _localNodeContextProvider = localNodeContextProvider;
        _globalVariablesRepository = globalVariablesRepository;
    }

    public void ExecuteBlockCreationCycle()
    {
        var currentTurn = _blockchainNodesRepository.CurrentWitnessTurn();
        if (currentTurn == null)
            return;

        var hotPoolTransactions = _hotPoolRepository.GetAllTransactions();
        var hotPoolSize = hotPoolTransactions.Count();
        var lastBlock = _blockchain.LastBlock;
        var timeSinceLastBlock = DateTime.UtcNow - lastBlock.Timestamp;

        //Is it my turn?
        if (currentTurn.NodeId.Equals(_localNodeContextProvider.LocalNodeId()))
        {
            //It is my turn!
            if (timeSinceLastBlock.TotalMinutes > MinutesToWaitBeforeCreatingNewBlock || hotPoolSize > HotPoolSizeThreshold)
                CreateNewBlockFromHotPool();
        }
        else
        {
            //It is not my turn!

            var hotPoolSizeWhenNodeWasSelectedVariable = _globalVariablesRepository.Get(HotPoolSizeWhenNodeWasSelectedGlobalVariableKey);
            var hotPoolSizeWhenNodeWasSelected = string.IsNullOrWhiteSpace(hotPoolSizeWhenNodeWasSelectedVariable.Value)
                ? 0
                : int.Parse(hotPoolSizeWhenNodeWasSelectedVariable.Value);

            //Was it already over 100 tx when the node was selected?
            if (hotPoolSizeWhenNodeWasSelected > HotPoolSizeThreshold)
            {
                //Yes

                //Is 5 minutes passed since the node is selected?
                if (DateTime.UtcNow > hotPoolSizeWhenNodeWasSelectedVariable.LastModified.AddMinutes(5))
                {
                    //Yes
                    VoteForNextNode(hotPoolSize);
                }
            }
            else
            {
                //No

                //Is 15 minutes passed since the node is selected?
                if (DateTime.UtcNow > hotPoolSizeWhenNodeWasSelectedVariable.LastModified.AddMinutes(15))
                {
                    //Yes
                    VoteForNextNode(hotPoolSize);
                }
            }
        }
    }

    private void CreateNewBlockFromHotPool()
    {
        if (BlockCreated == null)
            throw new InvalidOperationException("Cannot run this operation before subscribing for 'BlockCreated' event");

        var startTime = DateTime.Now;

        var transactions = _hotPoolRepository.GetAllTransactions().Take(MaximumNumberOfTransactionsInABlock).ToList();

        var block = _blockchain.CreateNewBlock(transactions);
        _hotPoolRepository.RemoveTransactions(transactions);
        _blockchainNodesRepository.ClearVotes();

        var duration = DateTime.Now - startTime;
        BlockCreated?.Invoke(this, new BlockCreatedEventArgs(block, duration));
    }

    public NodeIdValue VoteForNextNode()
    {
        var hotPoolTransactions = _hotPoolRepository.GetAllTransactions();
        var hotPoolSize = hotPoolTransactions.Count();

        return VoteForNextNode(hotPoolSize);
    }

    private NodeIdValue VoteForNextNode(int hotPoolSize)
    {
        // Choose next node (alphanumerical order)
        var nextNode = _blockchainNodesRepository.SelectNextWitnessNode();
        if (nextNode == null)
            return null;

        // Send the vote to the selected node
        // While the connection fails, select next node and send the vote to it again.
        var votePayload = VotePayload.Create(nextNode.NodeId);
        var header = Header.Create(Networks.Main, 1, nextNode.NodeId, votePayload.MessageType,
            votePayload.RawData.Length, votePayload.Sign(_localNodeContextProvider.LocalKeys.PrivateKey));
        var message = Message.Create(header, votePayload);
        while (!_blockchainNodeClient.Send(nextNode.VerifiedEndpoint1, message))
        {
            nextNode = _blockchainNodesRepository.SelectNextWitnessNode();
            if (nextNode == null)
                return null;
            votePayload = VotePayload.Create(nextNode.NodeId);
            header = Header.Create(Networks.Main, 1, nextNode.NodeId, votePayload.MessageType,
                votePayload.RawData.Length, votePayload.Sign(_localNodeContextProvider.LocalKeys.PrivateKey));
            message = Message.Create(header, votePayload);
        }

        _globalVariablesRepository.Set(HotPoolSizeWhenNodeWasSelectedGlobalVariableKey, hotPoolSize.ToString());

        // The selected node Id will be sent to all other witness nodes - So all witness nodes know(based on the majority of votes) who should create the next block.
        foreach (var node in _blockchainNodesRepository.GetWitnessNodes())
            _blockchainNodeClient.Send(node.VerifiedEndpoint1, message);

        return nextNode.NodeId;
    }
}