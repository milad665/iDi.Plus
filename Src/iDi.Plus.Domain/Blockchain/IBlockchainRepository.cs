using System.Collections.Generic;
using iDi.Blockchain.Framework.Blockchain;

namespace iDi.Plus.Domain.Blockchain;

public interface IBlockchainRepository<T> where T : ITransaction
{
    long GetLastBlockIndex();
    Block<T> GetLastBlock();
    List<string> GetHashesOfBlocksCreatedAfter(long blockIndex);
    List<string> GetHashesOfBlocksCreatedAfter(string blockHash);
    Block<T> GetBlock(string blockHash);
    Block<T> GetBlock(long blockIndex);
    void AddBlock(Block<T> block);
}