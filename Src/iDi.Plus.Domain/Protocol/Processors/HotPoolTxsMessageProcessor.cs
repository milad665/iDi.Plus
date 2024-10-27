using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Protocol.Processors;

public class HotPoolTxsMessageProcessor : MessageProcessorBase
{
    public HotPoolTxsMessageProcessor(IBlockchainNodeClient blockchainNodeClient,
        IIdBlockchainRepository idBlockchainRepository, IHotPoolRepository<IdTransaction> hotPoolRepository,
        ILocalNodeContextProvider localNodeContextProvider, IBlockchainNodesRepository blockchainNodesRepository) : base(
        blockchainNodeClient, idBlockchainRepository, hotPoolRepository, localNodeContextProvider,
        blockchainNodesRepository)
    {
    }

    public override MessageTypes MessageType => MessageTypes.HotPoolTxs;
    protected override Message ProcessPayload(Message message)
    {
        if (message.Payload is not HotPoolTxsPayload payload)
            throw new InvalidInputException("Payload can not be cast to the target type of this processor.");

        if (!LocalNodeContextProvider.IsWitnessNode) 
            return null;
        
        foreach (var hashValue in payload.Transactions)
        {
            var hotPoolTx = HotPoolRepository.GetTransaction(hashValue);
            if (hotPoolTx != null)
                continue;
                
            var getTxMessagePayload = GetTxPayload.Create(hashValue);
            var signature = SignPayload(getTxMessagePayload);
            var header = message.Header.ToResponseHeader(new NodeIdValue(LocalNodeContextProvider.LocalKeys.PublicKey),
                MessageTypes.GetBlock, getTxMessagePayload.RawData.Length, signature);
            var getBlockMessage = Message.Create(header, getTxMessagePayload);
            SendMessage(message.Header.NodeId, getBlockMessage);
        }

        return null;
    }
}