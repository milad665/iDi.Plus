using iDi.Blockchain.Framework.Cryptography;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Blockchain.IdTransactions;

public interface IIdTransactionFactory
{
    IdTransaction CreateFromTxDataResponsePayload(TxDataResponsePayload responsePayload);
    IdTransaction CreateFromTxDataRequestPayload(TxDataRequestPayload requestPayload, HashValue previousTransactionHash);
}