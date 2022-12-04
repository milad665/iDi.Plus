using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Blockchain.IdTransactions
{
    public sealed class ConsentIdTransaction : IdTransaction
    {
        public ConsentIdTransaction(AddressValue issuerAddress, AddressValue holderAddress, string subject, string identifierKey, string valueMimeType, byte[] doubleEncryptedData, AddressValue verifierAddress, HashValue previousTransactionHash) 
            : base(TransactionTypes.ConsentTransaction,issuerAddress, holderAddress, subject, identifierKey, valueMimeType, doubleEncryptedData, previousTransactionHash)
        {
            VerifierAddress = verifierAddress;

            TransactionHash = HashValue.ComputeHash(this);
        }

        public static ConsentIdTransaction FromTxDataResponsePayload(TxDataResponsePayload responsePayload)
        {
            if (responsePayload.TransactionType != TransactionTypes.ConsentTransaction)
                throw new InvalidInputException("Transaction type does not match.");

            return new ConsentIdTransaction(responsePayload.IssuerAddress, responsePayload.HolderAddress, responsePayload.Subject,
                responsePayload.IdentifierKey, responsePayload.ValueMimeType, responsePayload.DoubleEncryptedData, responsePayload.VerifierAddress,
                responsePayload.PreviousTransactionHash);
        }
        
        public static ConsentIdTransaction FromTxDataRequestPayload(TxDataRequestPayload requestPayload, HashValue previousTransactionHash)
        {
            if (requestPayload.TransactionType != TransactionTypes.ConsentTransaction)
                throw new InvalidInputException("Transaction type does not match.");

            return new ConsentIdTransaction(requestPayload.IssuerAddress, requestPayload.HolderAddress, requestPayload.Subject,
                requestPayload.IdentifierKey, requestPayload.ValueMimeType, requestPayload.DoubleEncryptedData, 
                requestPayload.VerifierAddress, previousTransactionHash);
        }

        public AddressValue VerifierAddress { get; set; }
    }
}