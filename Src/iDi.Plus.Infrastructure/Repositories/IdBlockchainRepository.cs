using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Entities;
using iDi.Plus.Infrastructure.Context;
using MongoDB.Driver;

namespace iDi.Plus.Infrastructure.Repositories;

public class IdBlockchainRepository : IIdBlockchainRepository
{
    private readonly IBlockchainContext _context;

    public IdBlockchainRepository(IBlockchainContext context)
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

    public List<HashValue> GetHashesOfBlocksCreatedAfter(HashValue blockHash)
    {
        var block = _context.Blocks.AsQueryable().FirstOrDefault(b => b.Hash.Equals(blockHash));
        if (block == null)
            return new List<HashValue>();

        return GetHashesOfBlocksCreatedAfter(block.Index);
    }

    public Block<IdTransaction> GetBlock(HashValue blockHash)
    {
        return _context.Blocks.AsQueryable().FirstOrDefault(b => b.Hash.Equals(blockHash));
    }

    public Block<IdTransaction> GetBlock(long blockIndex)
    {
        return _context.Blocks.FindSync(b => b.Index == blockIndex).FirstOrDefault();
    }

    public void AddBlock(Block<IdTransaction> block)
    {
        _context.Blocks.InsertOne(block);
    }

    public IssueIdTransaction GetLastIssueTransactionInTheVirtualTransactionChain(AddressValue issuerAddress, AddressValue holderAddress,
        string subject, string identifier)
    {
        // We need the NestedElement filter twice: once to filter documents and once to filter the array in the projection
        var nestedFilter = Builders<IdTransaction>.Filter.Eq(t => t.IssuerAddress, issuerAddress)
                           & Builders<IdTransaction>.Filter.Eq(t => t.HolderAddress, holderAddress)
                           & Builders<IdTransaction>.Filter.Eq(n => n.Subject, subject)
                           & Builders<IdTransaction>.Filter.Eq(n => n.IdentifierKey, identifier);


        var element = _context.Blocks
            .Find(
                Builders<Block<IdTransaction>>.Filter.ElemMatch(e => e.Transactions, nestedFilter)
            )
            .ToList()
            .SelectMany(e => e.Transactions)
            .Where(t => issuerAddress.Equals(t.IssuerAddress) && holderAddress.Equals(t.HolderAddress) && t.Subject.Equals(subject) &&
                        t.IdentifierKey.Equals(identifier)).OrderByDescending(t => t.Timestamp)
            .Cast<IssueIdTransaction>().FirstOrDefault();

        return element;

    }

    public IdTransaction GetTransaction(HashValue transactionHash)
    {
        var nestedFilter = Builders<IdTransaction>.Filter.Eq(t => t.TransactionHash, transactionHash);

        var element = _context.Blocks
            .Find(Builders<Block<IdTransaction>>.Filter.ElemMatch(e => e.Transactions, nestedFilter))
            .ToList()
            .SelectMany(e => e.Transactions)
            .FirstOrDefault(t => t.TransactionHash.Equals(transactionHash));

        return element;
    }

    public IdTransaction GetTransactionByPreviousTransactionHash(HashValue previousTransactionHash)
    {
        var nestedFilter = Builders<IdTransaction>.Filter.Eq(t => t.PreviousTransactionHash, previousTransactionHash);

        var element = _context.Blocks
            .Find(Builders<Block<IdTransaction>>.Filter.ElemMatch(e => e.Transactions, nestedFilter))
            .ToList()
            .SelectMany(e => e.Transactions)
            .FirstOrDefault(t => t.PreviousTransactionHash.Equals(previousTransactionHash));

        return element;
    }

    public IssueIdTransaction GetLastIssueTransactionInTheVirtualTransactionChainIncludingAddressChanges(List<AddressValue> issuerAddresses, 
        List<AddressValue> holderAddresses,
        string subject, string identifier)
    {
        // We need the NestedElement filter twice: once to filter documents and once to filter the array in the projection
        var nestedFilter = Builders<IdTransaction>.Filter.In(t => t.IssuerAddress, issuerAddresses)
                           & Builders<IdTransaction>.Filter.In(t => t.HolderAddress, holderAddresses)
                           & Builders<IdTransaction>.Filter.Eq(n => n.Subject, subject)
                           & Builders<IdTransaction>.Filter.Eq(n => n.IdentifierKey, identifier);


        var element = _context.Blocks
            .Find(Builders<Block<IdTransaction>>.Filter.ElemMatch(e => e.Transactions, nestedFilter))
            .ToList()
            .SelectMany(e => e.Transactions)
            .Where(t => issuerAddresses.Contains(t.IssuerAddress) && holderAddresses.Contains(t.HolderAddress) &&
                        t.Subject.Equals(subject) &&
                        t.IdentifierKey.Equals(identifier)).OrderByDescending(t => t.Timestamp)
            .Cast<IssueIdTransaction>().FirstOrDefault();

        return element;
    }

    public bool IsObsolete(AddressValue oldIdAddress)
    {
        return _context.KeyChanges.FindSync(k => k.OldAddress.Equals(oldIdAddress)).FirstOrDefault() != null;
    }

    public List<KeyChange> GetKeyChangeHistory(AddressValue currentIdAddress)
    {
        var result = new List<KeyChange>();
        var previousAddressChange = _context.KeyChanges.FindSync(k => k.NewAddress.Equals(currentIdAddress)).FirstOrDefault();
        while (previousAddressChange != null)
        {
            result.Add(previousAddressChange);
            var address = previousAddressChange.OldAddress;
            previousAddressChange = _context.KeyChanges.FindSync(k => k.NewAddress.Equals(address)).FirstOrDefault();
        }

        return result;
    }
}