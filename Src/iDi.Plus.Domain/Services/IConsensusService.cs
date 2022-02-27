using System;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Plus.Domain.Blockchain.IdTransactions;

namespace iDi.Plus.Domain.Services;

public interface IConsensusService
{
    public event Action<ConsensusService, BlockCreatedEventArgs> BlockCreated;

    void ExecuteBlockCreationCycle();
    NodeIdValue VoteForNextNode();
}

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