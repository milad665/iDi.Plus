using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Plus.Domain.Blockchain.IdTransactions;

namespace iDi.Plus.Domain.Blockchain;

public interface IIdBlockchainRepository : IBlockchainRepository<IdTransaction>
{
    IssueIdTransaction GetLastIssueTransactionInTheVirtualTransactionChain(AddressValue issuer, AddressValue holder, string subject, string identifier);
}