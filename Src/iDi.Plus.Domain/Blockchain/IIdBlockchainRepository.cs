using System.Collections.Generic;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Entities;

namespace iDi.Plus.Domain.Blockchain;

public interface IIdBlockchainRepository : IBlockchainRepository<IdTransaction>
{
    IssueIdTransaction GetLastIssueTransactionInTheVirtualTransactionChain(AddressValue issuerAddress, AddressValue holderAddress,
        string subject, string identifier);

    IdTransaction GetTransaction(HashValue transactionHash);
    IdTransaction GetTransactionByPreviousTransactionHash(HashValue previousTransactionHash);

    IssueIdTransaction GetLastIssueTransactionInTheVirtualTransactionChainIncludingAddressChanges(
        List<AddressValue> issuerAddresses, List<AddressValue> holderAddresses,
        string subject, string identifier);
    bool IsObsolete(AddressValue oldIdAddress);
    List<KeyChange> GetKeyChangeHistory(AddressValue currentIdAddress);
}