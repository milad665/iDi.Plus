using System.Collections.Generic;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Plus.Domain.Blockchain;

public interface IHotPoolRepository<T> where T : ITransaction
{
    IEnumerable<T> GetAllTransactions();
    T GetTransaction(HashValue hash);
    void AddTransaction(T transaction);
    void RemoveTransaction(T transaction);
    void RemoveTransaction(HashValue hash);
    void RemoveTransactions(IEnumerable<T> transactions);
}