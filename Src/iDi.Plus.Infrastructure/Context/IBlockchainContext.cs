using iDi.Blockchain.Framework.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using MongoDB.Driver;

namespace iDi.Plus.Infrastructure.Context;

public interface IBlockchainContext
{
    IMongoDatabase Database { get; }
    IMongoCollection<Block<IdTransaction>> Blocks { get; }
}