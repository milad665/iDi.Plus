using System.ComponentModel.DataAnnotations;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Protocol.Processors;

public class TxDataResponseMessageProcessor : MessageProcessorBase
{
    private IdTransactionFactory _idTransactionFactory;
    
    
    public TxDataResponseMessageProcessor(IBlockchainNodeClient blockchainNodeClient,
        IIdBlockchainRepository idBlockchainRepository, IHotPoolRepository<IdTransaction> hotPoolRepository,
        ILocalNodeContextProvider localNodeContextProvider, IBlockchainNodesRepository blockchainNodesRepository, IdTransactionFactory idTransactionFactory) : base(
        blockchainNodeClient, idBlockchainRepository, hotPoolRepository, localNodeContextProvider,
        blockchainNodesRepository)
    {
        _idTransactionFactory = idTransactionFactory;
    }

    public override MessageTypes MessageType => MessageTypes.TxDataResponse;
    protected override Message ProcessPayload(Message message)
    {
        if (message.Payload is not TxDataResponsePayload payload)
            throw new InvalidInputException("Payload can not be cast to the target type of this processor.");

        var idTx = _idTransactionFactory.CreateFromTxDataResponsePayload(payload);
        
        //Verify TX
        //Add TX to hot pool
        
        throw new System.NotImplementedException();
    }
}