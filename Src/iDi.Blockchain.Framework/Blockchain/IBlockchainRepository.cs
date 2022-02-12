using System.Collections.Generic;
using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Blockchain.Framework.Blockchain;

public interface IBlockchainRepository<T> where T : ITransaction
{
    long GetLastBlockIndex();
    Block<T> GetLastBlock();
    List<HashValue> GetHashesOfBlocksCreatedAfter(long blockIndex);
    List<HashValue> GetHashesOfBlocksCreatedAfter(string blockHash);
    Block<T> GetBlock(string blockHash);
    Block<T> GetBlock(long blockIndex);
    void AddBlock(Block<T> block);
}