using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Protocol.Processors;

public class NewBlocksMessageProcessor : MessageProcessorBase
{
    public NewBlocksMessageProcessor(IBlockchainNodeClient blockchainNodeClient,
        IIdBlockchainRepository idBlockchainRepository, IHotPoolRepository<IdTransaction> hotPoolRepository,
        ILocalNodeContextProvider localNodeContextProvider, IBlockchainNodesRepository blockchainNodesRepository) : base(
        blockchainNodeClient, idBlockchainRepository, hotPoolRepository, localNodeContextProvider,
        blockchainNodesRepository)
    {
    }

    public override MessageTypes MessageType => MessageTypes.NewBlocks;

    protected override Message ProcessPayload(Message message)
    {
        if (message.Payload is not NewBlocksPayload payload)
            throw new InvalidInputException("Payload can not be cast to the target type of this processor.");

        foreach (var blockHash in payload.Blocks)
        {
            var getBlockMessagePayload = GetBlockPayload.Create(blockHash);
            var signature = SignPayload(getBlockMessagePayload);
            var header = message.Header.ToResponseHeader(new NodeIdValue(LocalNodeContextProvider.LocalKeys.PublicKey),
                MessageTypes.GetBlock, getBlockMessagePayload.RawData.Length, signature);
            var getBlockMessage = Message.Create(header, getBlockMessagePayload);
            SendMessage(message.Header.NodeId, getBlockMessage);
        }

        return null;
    }
}