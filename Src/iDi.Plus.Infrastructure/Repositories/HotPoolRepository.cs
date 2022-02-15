using iDi.Blockchain.Framework.Cryptography;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Infrastructure.Context;
using MongoDB.Driver;

namespace iDi.Plus.Infrastructure.Repositories;

public class HotPoolRepository : IHotPoolRepository<IdTransaction>
{
    private readonly IBlockchainContext _context;

    public HotPoolRepository(IBlockchainContext context)
    {
        _context = context;
    }

    public IEnumerable<IdTransaction> GetAllTransactions()
    {
        return _context.HotPoolTransactions.AsQueryable().ToList();
    }

    public IdTransaction GetTransaction(HashValue hash)
    {
        return _context.HotPoolTransactions.AsQueryable().FirstOrDefault(t =>
            t.TransactionHash.HexString.Equals(hash.HexString, StringComparison.OrdinalIgnoreCase));
    }

    public void AddTransaction(IdTransaction transaction)
    {
        _context.HotPoolTransactions.InsertOne(transaction);
    }

    public void RemoveTransaction(IdTransaction transaction)
    {
        _context.HotPoolTransactions.DeleteOne(t =>
            t.TransactionHash.Equals(transaction.TransactionHash));
    }

    public void RemoveTransaction(HashValue hash)
    {
        _context.HotPoolTransactions.DeleteOne(t => t.TransactionHash.Equals(hash));
    }

    public void RemoveTransactions(IEnumerable<IdTransaction> transactions)
    {
        var txHashes = transactions.Select(t => t.TransactionHash).ToList();

        _context.HotPoolTransactions.DeleteMany(x => txHashes.Contains(x.TransactionHash));
    }
}