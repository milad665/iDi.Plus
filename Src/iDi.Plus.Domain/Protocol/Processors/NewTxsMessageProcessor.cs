using System.ComponentModel.DataAnnotations.Schema;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Protocol.Processors;

public class NewTxsMessageProcessor : MessageProcessorBase
{
    public NewTxsMessageProcessor(IBlockchainNodeClient blockchainNodeClient,
        IIdBlockchainRepository idBlockchainRepository, IHotPoolRepository<IdTransaction> hotPoolRepository,
        ILocalNodeContextProvider localNodeContextProvider, IBlockchainNodesRepository blockchainNodesRepository) : base(
        blockchainNodeClient, idBlockchainRepository, hotPoolRepository, localNodeContextProvider,
        blockchainNodesRepository)
    {
    }

    public override MessageTypes MessageType => MessageTypes.NewTxs;
    protected override Message ProcessPayload(Message message)
    {
        if (message.Payload is not NewTxsPayload payload)
            throw new InvalidInputException("Payload can not be cast to the target type of this processor.");

        foreach (var hashValue in payload.Transactions)
        {
            
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