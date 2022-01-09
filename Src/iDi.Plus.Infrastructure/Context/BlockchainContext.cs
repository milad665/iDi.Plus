using iDi.Blockchain.Framework.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace iDi.Plus.Infrastructure.Context;

public class BlockchainContext : IBlockchainContext
{
    public BlockchainContext(string connectionString)
    {
        var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
        var client = new MongoClient(clientSettings);
        Database = client.GetDatabase("IdPlusBlockchain");
        Blocks = Database.GetCollection<Block<IdTransaction>>(nameof(Blocks));

        var transactionHashIndexKeysDefinition = Builders<Block<IdTransaction>>.IndexKeys.Ascending("Transactions.TransactionHash");
        var blockIndexIndexKeysDefinition = Builders<Block<IdTransaction>>.IndexKeys.Ascending(b => b.Index);
        var blockHashIndexKeysDefinition = Builders<Block<IdTransaction>>.IndexKeys.Ascending(b => b.Hash);

        Blocks.Indexes.CreateOne(new CreateIndexModel<Block<IdTransaction>>(transactionHashIndexKeysDefinition));
        Blocks.Indexes.CreateOne(new CreateIndexModel<Block<IdTransaction>>(blockIndexIndexKeysDefinition));
        Blocks.Indexes.CreateOne(new CreateIndexModel<Block<IdTransaction>>(blockHashIndexKeysDefinition));
        
        BsonClassMap.RegisterClassMap<IdTransaction>(cm => {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });
        BsonClassMap.RegisterClassMap<IssueIdTransaction>();
        BsonClassMap.RegisterClassMap<ConsentIdTransaction>();
    }

    public IMongoDatabase Database { get; }
    public IMongoCollection<Block<IdTransaction>> Blocks { get; }
}