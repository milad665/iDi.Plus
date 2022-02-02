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
        HotPoolTransactions = Database.GetCollection<IdTransaction>(nameof(HotPoolTransactions));

        var blockTransactionHashIndexKeysDefinition = Builders<Block<IdTransaction>>.IndexKeys.Ascending("Transactions.TransactionHash");
        var blockTransactionHolderIndexKeysDefinition = Builders<Block<IdTransaction>>.IndexKeys.Ascending("Transactions.HolderAddress");
        var blockTransactionSubjectIndexKeysDefinition = Builders<Block<IdTransaction>>.IndexKeys.Ascending("Transactions.Subject");
        var blockIndexIndexKeysDefinition = Builders<Block<IdTransaction>>.IndexKeys.Ascending(b => b.Index);
        var blockHashIndexKeysDefinition = Builders<Block<IdTransaction>>.IndexKeys.Ascending(b => b.Hash);

        Blocks.Indexes.CreateOne(new CreateIndexModel<Block<IdTransaction>>(blockTransactionHashIndexKeysDefinition));
        Blocks.Indexes.CreateOne(new CreateIndexModel<Block<IdTransaction>>(blockTransactionHolderIndexKeysDefinition));
        Blocks.Indexes.CreateOne(new CreateIndexModel<Block<IdTransaction>>(blockTransactionSubjectIndexKeysDefinition));
        Blocks.Indexes.CreateOne(new CreateIndexModel<Block<IdTransaction>>(blockIndexIndexKeysDefinition));
        Blocks.Indexes.CreateOne(new CreateIndexModel<Block<IdTransaction>>(blockHashIndexKeysDefinition));


        var transactionHashIndexKeysDefinition = Builders<IdTransaction>.IndexKeys.Ascending(t => t.TransactionHash);
        var transactionTimestampIndexKeysDefinition = Builders<IdTransaction>.IndexKeys.Ascending(t => t.Timestamp);

        HotPoolTransactions.Indexes.CreateOne(new CreateIndexModel<IdTransaction>(transactionHashIndexKeysDefinition));
        HotPoolTransactions.Indexes.CreateOne(new CreateIndexModel<IdTransaction>(transactionTimestampIndexKeysDefinition));

        BsonClassMap.RegisterClassMap<IdTransaction>(cm => {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });
        BsonClassMap.RegisterClassMap<IssueIdTransaction>();
        BsonClassMap.RegisterClassMap<ConsentIdTransaction>();
    }

    public IMongoDatabase Database { get; }
    public IMongoCollection<Block<IdTransaction>> Blocks { get; }
    public IMongoCollection<IdTransaction> HotPoolTransactions { get; }
}