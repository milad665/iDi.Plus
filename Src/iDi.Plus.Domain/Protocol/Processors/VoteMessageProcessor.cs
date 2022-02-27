using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Protocol.Processors;

public class VoteMessageProcessor : MessageProcessorBase
{
    public VoteMessageProcessor(IBlockchainNodeClient blockchainNodeClient,
        IIdBlockchainRepository idBlockchainRepository,
        IHotPoolRepository<IdTransaction> hotPoolRepository,
        ILocalNodeContextProvider localNodeContextProvider,
        IBlockchainNodesRepository blockchainNodesRepository) : base(blockchainNodeClient, idBlockchainRepository,
        hotPoolRepository, localNodeContextProvider, blockchainNodesRepository)
    {
    }

    public override MessageTypes MessageType => MessageTypes.Vote;

    public override Message ProcessPayload(Message message)
    {
        if (message.Payload is not VotePayload payload)
            throw new InvalidInputException("Payload can not be cast to the target type of this processor.");

        BlockchainNodesRepository.SetWitnessNodeVote(message.Header.NodeId, payload.NodeId);

        return null;
    }
}