using iDi.Blockchain.Framework.Blockchain;
using iDi.Plus.Infrastructure.Context;
using MongoDB.Driver;

namespace iDi.Plus.Infrastructure.Repositories;

public class BlockchainRepository : IBlockchainRepository
{
    private readonly IBlockchainContext _context;

    public BlockchainRepository(IBlockchainContext context)
    {
        _context = context;
    }

    public DateTime GetLastBlockTimestamp()
    {
        var lastBlock = _context.Blocks.AsQueryable().OrderByDescending(b => b.Timestamp).FirstOrDefault();
        return lastBlock?.Timestamp ?? DateTime.MinValue;
    }
}