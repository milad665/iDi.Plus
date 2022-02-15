using iDi.Blockchain.Framework.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;

namespace iDi.Plus.Domain.Services;

public interface IConsensusService
{
    void CreateNewBlockFromHotPool();
    void VerifyBlock(Block<IdTransaction> block);
    void VerifyTransaction(IdTransaction transaction);

}