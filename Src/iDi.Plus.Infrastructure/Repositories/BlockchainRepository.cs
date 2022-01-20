using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Infrastructure.Context;
using MongoDB.Driver;

namespace iDi.Plus.Infrastructure.Repositories;

public class BlockchainRepository : IBlockchainRepository<IdTransaction>
{
    private readonly IBlockchainContext _context;

    public BlockchainRepository(IBlockchainContext context)
    {
        _context = context;
    }

    public long GetLastBlockIndex()
    {
        var lastBlock = _context.Blocks.AsQueryable().OrderByDescending(b => b.Index).FirstOrDefault();
        return lastBlock?.Index ?? -1;
    }

    public Block<IdTransaction> GetLastBlock()
    {
        var block = _context.Blocks.AsQueryable().OrderByDescending(b => b.Index).FirstOrDefault();
        return block;
    }

    public List<HashValue> GetHashesOfBlocksCreatedAfter(long blockIndex)
    {
        return _context.Blocks.FindSync(b => b.Index > blockIndex).ToList().Select(b => b.Hash).ToList();
    }

    public List<HashValue> GetHashesOfBlocksCreatedAfter(string blockHash)
    {
        var block = _context.Blocks.AsQueryable().FirstOrDefault(b => b.Hash.Equals(blockHash));
        if (block == null)
            return new List<HashValue>();

        return GetHashesOfBlocksCreatedAfter(block.Index);
    }

    public Block<IdTransaction> GetBlock(string blockHash)
    {
        return _context.Blocks.AsQueryable().FirstOrDefault(b => b.Hash.Equals(blockHash));
    }

    public Block<IdTransaction> GetBlock(long blockIndex)
    {
        return _context.Blocks.AsQueryable().FirstOrDefault(b => b.Index == blockIndex);

    }

    public void AddBlock(Block<IdTransaction> block)
    {
        _context.Blocks.InsertOne(block);
    }
}