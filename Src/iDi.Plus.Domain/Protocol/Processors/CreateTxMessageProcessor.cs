using System;
using System.Linq;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Protocol.Processors;

public class CreateTxMessageProcessor : MessageProcessorBase
{
    private readonly IIdTransactionFactory _idTransactionFactory;

    public CreateTxMessageProcessor(IBlockchainNodeClient blockchainNodeClient,
        IBlockchainRepository<IdTransaction> blockchainRepository, IHotPoolRepository<IdTransaction> hotPoolRepository,
        ILocalNodeContextProvider localNodeContextProvider,
        IBlockchainNodesProvider blockchainNodesProvider, IIdTransactionFactory idTransactionFactory)
        : base(blockchainNodeClient, blockchainRepository, hotPoolRepository, localNodeContextProvider,
            blockchainNodesProvider)
    {
        _idTransactionFactory = idTransactionFactory;
    }

    public override MessageTypes MessageType => MessageTypes.CreateTx;
    public override Message ProcessPayload(Message message)
    {
        if (message.Payload is not CreateTxPayload payload)
            throw new InvalidInputException("Payload can not be cast to the target type of this processor.");

        //If the current node is a witness node verify the message and store in hot pool before forwarding to other witness nodes
        if (LocalNodeContextProvider.IsWitnessNode)
        {
            var transactionInHotPool = HotPoolRepository.GetTransaction(payload.TxDataPayload.TransactionHash);
            if (transactionInHotPool != null)
                return null;

            var idTransaction = _idTransactionFactory.CreateFromTxDataPayload(payload.TxDataPayload);
            if (idTransaction == null)
                throw new InvalidInputException("Id transaction cannot be created from the payload data.");

            idTransaction.Verify();
            HotPoolRepository.AddTransaction(idTransaction);
        }

        var witnessNodes = BlockchainNodesProvider.AllNodes()
            .Where(n => n.IsWitnessNode && !n.NodeId.Equals(message.Header.NodeId)).ToList();

        foreach (var node in witnessNodes)
                SendToNode(node.NodeId, message);

        return null;
    }
}