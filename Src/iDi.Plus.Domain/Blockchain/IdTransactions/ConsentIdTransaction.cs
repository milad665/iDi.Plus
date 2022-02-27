using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Blockchain.IdTransactions
{
    public sealed class ConsentIdTransaction : IdTransaction
    {
        public ConsentIdTransaction(AddressValue issuerAddress, AddressValue holderAddress, string subject, string identifierKey, byte[] signedData, AddressValue verifierAddress, HashValue previousTransactionHash) 
            : base(TransactionTypes.ConsentTransaction,issuerAddress, holderAddress, subject, identifierKey, signedData, previousTransactionHash)
        {
            VerifierAddress = verifierAddress;

            TransactionHash = HashValue.ComputeHash(this);
        }

        public static ConsentIdTransaction FromTxDataPayload(TxDataPayload payload)
        {
            if (payload.TransactionType != TransactionTypes.ConsentTransaction)
                throw new InvalidInputException("Transaction type does not match.");

            return new ConsentIdTransaction(payload.IssuerAddress, payload.HolderAddress, payload.Subject,
                payload.IdentifierKey, payload.DoubleEncryptedData, payload.VerifierAddress,
                payload.PreviousTransactionHash);
        }

        public AddressValue VerifierAddress { get; set; }
    }
}