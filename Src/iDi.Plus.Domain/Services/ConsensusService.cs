using System;
using System.Linq;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;

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


    public const int MaximumNumberOfTransactionsInABlock = 1000;
    public event Action<ConsensusService, BlockCreatedEventArgs> BlockCreated;

    public ConsensusService(IHotPoolRepository<IdTransaction> hotPoolRepository, IBlockchain<IdTransaction> blockchain)
    {
        _hotPoolRepository = hotPoolRepository;
        _blockchain = blockchain;
    }

    public void CreateNewBlockFromHotPool()
    {
        if (BlockCreated == null)
            throw new InvalidOperationException("Cannot run this operation before subscribing for 'BlockCreated' event");

        var startTime = DateTime.Now;

        var transactions = _hotPoolRepository.GetAllTransactions().Take(MaximumNumberOfTransactionsInABlock).ToList();

        var block = _blockchain.CreateBlock(transactions);
        _hotPoolRepository.RemoveTransactions(transactions);
        
        var duration = DateTime.Now - startTime;
        BlockCreated?.Invoke(this, new BlockCreatedEventArgs(block, duration));
    }
}