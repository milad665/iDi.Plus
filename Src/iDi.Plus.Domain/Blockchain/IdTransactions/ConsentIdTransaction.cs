using System.Security.Cryptography;
using System.Text;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Blockchain.IdTransactions
{
    public sealed class ConsentIdTransaction : IdTransaction
    {
        public ConsentIdTransaction(string issuerAddress, string holderAddress, string subject, string identifierKey, string signedData, string verifierPublicKey, string previousTransactionHash) 
            : base(TransactionTypes.ConsentTransaction,issuerAddress, holderAddress, subject, identifierKey, signedData, previousTransactionHash)
        {
            VerifierPublicKey = verifierPublicKey;

            TransactionHash = ComputeHash();
        }

        public static ConsentIdTransaction FromTxDataPayload(TxDataPayload payload)
        {
            if (payload.TransactionType != TransactionTypes.ConsentTransaction)
                throw new InvalidDataException("Transaction type does not match.");

            return new ConsentIdTransaction(payload.IssuerAddress, payload.HolderAddress, payload.Subject,
                payload.IdentifierKey, payload.SignedData.ToHexString(), payload.VerifierAddress,
                payload.PreviousTransactionHash);
        }

        public string VerifierPublicKey { get; set; }
        public override string ComputeHash()
        {
            var tx =
                $"{TransactionType}:{IssuerAddress}:{HolderAddress}:{VerifierPublicKey}:{IdentifierKey}:{Timestamp:yyMMddHHmmss.FFFFFFF}:{SignedData}";
            var bytes = Encoding.UTF8.GetBytes(tx);
            using var sha256Hash = SHA256.Create();
            var hashedBytes = sha256Hash.ComputeHash(bytes);
            return Encoding.UTF8.GetString(hashedBytes);
        }
    }
}