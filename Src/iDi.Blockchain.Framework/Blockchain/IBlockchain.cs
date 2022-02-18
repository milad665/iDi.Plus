using System.Collections.Generic;

namespace iDi.Blockchain.Framework.Blockchain;

public interface IBlockchain<TTransaction> where TTransaction : ITransaction
{
    Block<TTransaction> LastBlock { get; }
    long BlocksCount { get; }
    int Difficulty { get; }
    Block<TTransaction> CreateBlock(List<TTransaction> transactions);
    void VerifyBlock(Block<TTransaction> block);
    void VerifyTransaction(TTransaction transaction);
}