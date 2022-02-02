using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Blockchain.IdTransactions;

public interface IIdTransactionFactory
{
    IdTransaction CreateFromTxDataPayload(TxDataPayload payload);
}