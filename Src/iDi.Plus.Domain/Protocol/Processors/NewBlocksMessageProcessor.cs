using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Protocol.Processors;

public class NewBlocksMessageProcessor : MessageProcessorBase
{
    public NewBlocksMessageProcessor(IBlockchainNodeClient blockchainNodeClient, IBlockchainRepository<IdTransaction> blockchainRepository, 
        ILocalNodeContextProvider localNodeContextProvider, BlockchainNodesProvider blockchainNodesProvider) 
        : base(blockchainNodeClient, blockchainRepository, localNodeContextProvider, blockchainNodesProvider)
    {
    }

    public override MessageTypes MessageType => MessageTypes.NewBlocks;

    public override Message ProcessPayload(Message message)
    {
        var payload = message.Payload as NewBlocksPayload;
        if (payload == null)
            throw new InvalidDataException("Payload can not be cast to the target type of this processor.");

        foreach (var blockHash in payload.Blocks)
        {
            var getBlockMessagePayload = GetBlockPayload.Create(blockHash);
            var signature = SignPayload(getBlockMessagePayload);
            var header = message.Header.ToResponseHeader(LocalNodeContextProvider.LocalKeys.PublicKey.ToHexString(),
                MessageTypes.GetBlock, getBlockMessagePayload.RawData.Length, signature);
            var getBlockMessage = Message.Create(header, getBlockMessagePayload);
            SendMessage(message.Header.NodeId, getBlockMessage);
        }

        return null;
    }
}