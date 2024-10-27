using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Protocol.Processors;

public class RequestBlockchainUpdateMessageProcessor : MessageProcessorBase
{
    protected RequestBlockchainUpdateMessageProcessor()
    {}
    
    public RequestBlockchainUpdateMessageProcessor(IBlockchainNodeClient blockchainNodeClient,
        IIdBlockchainRepository idBlockchainRepository, IHotPoolRepository<IdTransaction> hotPoolRepository,
        ILocalNodeContextProvider localNodeContextProvider, IBlockchainNodesRepository blockchainNodesRepository) : base(
        blockchainNodeClient, idBlockchainRepository, hotPoolRepository, localNodeContextProvider,
        blockchainNodesRepository)
    {
    }

    public override MessageTypes MessageType => MessageTypes.RequestBlockchainUpdate;

    protected override Message ProcessPayload(Message message)
    {
        if (message.Payload is not RequestBlockchainUpdatePayload payload)
            throw new InvalidInputException("Payload can not be cast to the target type of this processor.");

        var hashes = IdBlockchainRepository.GetHashesOfBlocksCreatedAfter(payload.LastBlockIndex);
        var returnPayload = hashes is {Count: > 0} ? NewBlocksPayload.Create(hashes) : null;

        var signature = SignPayload(returnPayload);
        var header = Header.Create(payload.Network, payload.Version, message.Header.NodeId,
            returnPayload?.MessageType ?? MessageTypes.Empty, returnPayload?.RawData?.Length ?? 0, signature);
        return Message.Create(header, returnPayload);
    }
}