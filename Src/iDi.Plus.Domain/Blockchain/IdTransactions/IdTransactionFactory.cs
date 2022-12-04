using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Blockchain.IdTransactions;

public class IdTransactionFactory : IIdTransactionFactory
{
    public IdTransaction CreateFromTxDataResponsePayload(TxDataResponsePayload responsePayload)
    {
        if (responsePayload == null)
            return null;

        return responsePayload.TransactionType switch
        {
            TransactionTypes.IssueTransaction => IssueIdTransaction.FromTxDataResponsePayload(responsePayload),
            TransactionTypes.ConsentTransaction => ConsentIdTransaction.FromTxDataResponsePayload(responsePayload),
            _ => throw new InvalidInputException("Unsupported Transaction Type")
        };
    }

    public IdTransaction CreateFromTxDataRequestPayload(TxDataRequestPayload requestPayload, HashValue previousTransactionHash)
    {
        if (requestPayload == null)
            return null;

        return requestPayload.TransactionType switch
        {
            TransactionTypes.IssueTransaction => IssueIdTransaction.FromTxDataRequestPayload(requestPayload, previousTransactionHash),
            TransactionTypes.ConsentTransaction => ConsentIdTransaction.FromTxDataRequestPayload(requestPayload, previousTransactionHash),
            _ => throw new InvalidInputException("Unsupported Transaction Type")
        };
    }
}