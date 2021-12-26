using System.Security.Cryptography;
using System.Text;
using iDi.Blockchain.Framework;

namespace iDi.Plus.Domain.Blockchain.IdTransactions
{
    public sealed class IssueIdTransaction : IdTransaction
    {
        public IssueIdTransaction(string issuerAddress, string holderAddress, string subject, string identifierKey, string signedData, string previousTransactionHash) 
            : base(TransactionTypes.IssueTransaction, issuerAddress, holderAddress, subject, identifierKey, signedData, previousTransactionHash)
        {
            

            TransactionHash = ComputeHash();
        }

        public override string ComputeHash()
        {
            var tx =
                $"{TransactionType}:{IssuerAddress}:{HolderAddress}:{IdentifierKey}:{Timestamp:yyMMddHHmmss.FFFFFFF}:{SignedData}";
            var bytes = Encoding.UTF8.GetBytes(tx);
            using var sha256Hash = SHA256.Create();
            var hashedBytes = sha256Hash.ComputeHash(bytes);
            return Encoding.UTF8.GetString(hashedBytes);
        }
    }
}