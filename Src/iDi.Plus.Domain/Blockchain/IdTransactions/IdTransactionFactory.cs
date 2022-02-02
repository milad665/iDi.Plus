using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Blockchain.IdTransactions;

public class IdTransactionFactory : IIdTransactionFactory
{
    public IdTransaction CreateFromTxDataPayload(TxDataPayload payload)
    {
        if (payload == null)
            return null;

        return payload.TransactionType switch
        {
            TransactionTypes.IssueTransaction => IssueIdTransaction.FromTxDataPayload(payload),
            TransactionTypes.ConsentTransaction => ConsentIdTransaction.FromTxDataPayload(payload),
            _ => throw new InvalidInputException("Unsupported Transaction Type")
        };
    }
}