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
            t.TransactionHash.HexString.Equals(transaction.TransactionHash.HexString,
                StringComparison.OrdinalIgnoreCase));
    }

    public void RemoveTransaction(HashValue hash)
    {
        _context.HotPoolTransactions.DeleteOne(t =>
            t.TransactionHash.HexString.Equals(hash.HexString,
                StringComparison.OrdinalIgnoreCase));
    }
}