using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Protocol.Processors;

public class BlockDataMessageProcessor : MessageProcessorBase
{
    private readonly IBlockchain<IdTransaction> _blockchain;
    private readonly IIdTransactionFactory _idTransactionFactory;

    public BlockDataMessageProcessor(IBlockchainNodeClient blockchainNodeClient,
        IIdBlockchainRepository idBlockchainRepository,
        IHotPoolRepository<IdTransaction> hotPoolRepository,
        ILocalNodeContextProvider localNodeContextProvider,
        IBlockchainNodesRepository blockchainNodesRepository, 
        IBlockchain<IdTransaction> blockchain, 
        IIdTransactionFactory idTransactionFactory) : base(blockchainNodeClient, idBlockchainRepository,
        hotPoolRepository, localNodeContextProvider, blockchainNodesRepository)
    {
        _blockchain = blockchain;
        _idTransactionFactory = idTransactionFactory;
    }

    public override MessageTypes MessageType => MessageTypes.BlockData;
    public override Message ProcessPayload(Message message)
    {
        if (message.Payload is not BlockDataPayload payload)
            throw new InvalidInputException("Payload can not be cast to the target type of this processor.");

        var block = payload.ToBlock(_idTransactionFactory);
        _blockchain.AddReceivedBlock(block);

        throw new System.NotImplementedException();
    }
}