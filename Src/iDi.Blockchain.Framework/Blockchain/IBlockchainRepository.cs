using System;

namespace iDi.Blockchain.Framework.Blockchain;

public interface IBlockchainRepository
{
    DateTime GetLastBlockTimestamp();
}