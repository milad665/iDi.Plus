using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Plus.Domain.Services;

public interface IConsensusService
{
    void CreateNewBlockFromHotPool();
    void ExecuteBlockCreationCycle();
    NodeIdValue VoteForNextNode();
}