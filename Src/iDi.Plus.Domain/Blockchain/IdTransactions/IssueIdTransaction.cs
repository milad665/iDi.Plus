using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Blockchain.IdTransactions
{
    public sealed class IssueIdTransaction : IdTransaction
    {
        public IssueIdTransaction(AddressValue issuerAddress, AddressValue holderAddress, string subject, string identifierKey, string valueMimeType, byte[] doubleEncryptedData, HashValue previousTransactionHash) 
            : base(TransactionTypes.IssueTransaction, issuerAddress, holderAddress, subject, identifierKey, valueMimeType, doubleEncryptedData, previousTransactionHash)
        {
            TransactionHash = HashValue.ComputeHash(this);
        }

        public static IssueIdTransaction FromTxDataResponsePayload(TxDataResponsePayload responsePayload)
        {
            if (responsePayload.TransactionType != TransactionTypes.IssueTransaction)
                throw new InvalidInputException("Transaction type does not match.");

            return new IssueIdTransaction(responsePayload.IssuerAddress, responsePayload.HolderAddress, responsePayload.Subject,
                responsePayload.IdentifierKey,  responsePayload.ValueMimeType, responsePayload.DoubleEncryptedData, responsePayload.PreviousTransactionHash);
        }
        
        public static IssueIdTransaction FromTxDataRequestPayload(TxDataRequestPayload requestPayload, HashValue previousTransactionHash)
        {
            if (requestPayload.TransactionType != TransactionTypes.IssueTransaction)
                throw new InvalidInputException("Transaction type does not match.");

            return new IssueIdTransaction(requestPayload.IssuerAddress, requestPayload.HolderAddress, requestPayload.Subject,
                requestPayload.IdentifierKey,  requestPayload.ValueMimeType, requestPayload.DoubleEncryptedData, previousTransactionHash);
        }
    }
}