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
    public class BlockCreatedEventArgs
    {
        public BlockCreatedEventArgs(Block<IdTransaction> block, TimeSpan processDuration)
        {
            Block = block;
            ProcessDuration = processDuration;
        }

        public Block<IdTransaction> Block { get; }
        public TimeSpan ProcessDuration { get; }
    }

    private readonly IBlockchain<IdTransaction> _blockchain;
    private readonly IHotPoolRepository<IdTransaction> _hotPoolRepository;
    private readonly IBlockchainNodesRepository _blockchainNodesRepository;
    private readonly IBlockchainNodeClient _blockchainNodeClient;
    private readonly ILocalNodeContextProvider _localNodeContextProvider;

    public const int MaximumNumberOfTransactionsInABlock = 1000;
    public event Action<ConsensusService, BlockCreatedEventArgs> BlockCreated;

    public ConsensusService(IHotPoolRepository<IdTransaction> hotPoolRepository, 
        IBlockchain<IdTransaction> blockchain,
        IBlockchainNodesRepository blockchainNodesRepository, 
        IBlockchainNodeClient blockchainNodeClient, 
        ILocalNodeContextProvider localNodeContextProvider)
    {
        _hotPoolRepository = hotPoolRepository;
        _blockchain = blockchain;
        _blockchainNodesRepository = blockchainNodesRepository;
        _blockchainNodeClient = blockchainNodeClient;
        _localNodeContextProvider = localNodeContextProvider;
    }

    public void CreateNewBlockFromHotPool()
    {
        if (BlockCreated == null)
            throw new InvalidOperationException("Cannot run this operation before subscribing for 'BlockCreated' event");

        var startTime = DateTime.Now;

        var transactions = _hotPoolRepository.GetAllTransactions().Take(MaximumNumberOfTransactionsInABlock).ToList();

        var block = _blockchain.CreateNewBlock(transactions);
        _hotPoolRepository.RemoveTransactions(transactions);
        
        var duration = DateTime.Now - startTime;
        BlockCreated?.Invoke(this, new BlockCreatedEventArgs(block, duration));
    }

    public void ExecuteBlockCreationCycle()
    {
        //Is it my turn
        //  Yes:
        //      - Is 5 minutes passed since the last block creation or the there are 100+ transaction in the Hot Pool?
        //          - Yes:
        //              Create a block
        //              Send to other nodes
        //          - No:
        //              Wait
        //  No:
        //      - Have I voted?
        //          - Yes:
        //              - Was it already over 100 tx when the node was selected?
        //                  - Yes:
        //                      - Is 5 minutes passed since the node is selected?
        //                          - Yes:
        //                              Revoke the turn from the node
        //                              Vote again
        //                          - No:
        //                              Wait
        //                  -No:
        //                      - Is 15 minutes passed since the node is selected?
        //                          - Yes:
        //                              Revoke the turn from the node
        //                              Vote again
        //                          - No:
        //                              Wait
        //          - No:
        //              Wait


        throw new NotImplementedException();
    }

    public NodeIdValue VoteForNextNode()
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

        // The selected node Id will be sent to all other witness nodes - So all witness nodes know(based on the majority of votes) who should create the next block.
        foreach (var node in _blockchainNodesRepository.GetWitnessNodes())
            _blockchainNodeClient.Send(node.VerifiedEndpoint1, message);

        return nextNode.NodeId;
    }
}