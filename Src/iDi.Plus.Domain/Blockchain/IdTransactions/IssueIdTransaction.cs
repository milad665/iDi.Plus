using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Blockchain.IdTransactions
{
    public sealed class IssueIdTransaction : IdTransaction
    {
        public IssueIdTransaction(string issuerAddress, string holderAddress, string subject, string identifierKey, string signedData, HashValue previousTransactionHash) 
            : base(TransactionTypes.IssueTransaction, issuerAddress, holderAddress, subject, identifierKey, signedData, previousTransactionHash)
        {
            TransactionHash = HashValue.ComputeHash(this);
        }

        public static IssueIdTransaction FromTxDataPayload(TxDataPayload payload)
        {
            if (payload.TransactionType != TransactionTypes.IssueTransaction)
                throw new InvalidInputException("Transaction type does not match.");

            return new IssueIdTransaction(payload.IssuerAddress, payload.HolderAddress, payload.Subject,
                payload.IdentifierKey, payload.DoubleEncryptedData.ToHexString(), payload.PreviousTransactionHash);
        }
    }
}