using System.Security.Cryptography;
using System.Text;

namespace iDi.Plus.Domain.Blockchain.IdTransactions
{
    public sealed class IssueIdTransaction : IdTransaction
    {
        private IssueIdTransaction()
        {}

        public IssueIdTransaction(string issuerPublicKey, string holderPublicKey, string identifierKey, string signedData, string previousTransactionHash) 
            : base(TransactionTypes.IssueTransaction, issuerPublicKey, holderPublicKey, identifierKey, signedData, previousTransactionHash)
        {
            

            TransactionHash = ComputeHash();
        }

        public override string ComputeHash()
        {
            var tx =
                $"{TransactionType}:{IssuerPublicKey}:{HolderPublicKey}:{IdentifierKey}:{Timestamp:yyMMddHHmmss.FFFFFFF}:{SignedData}";
            var bytes = Encoding.UTF8.GetBytes(tx);
            using var sha256Hash = SHA256.Create();
            var hashedBytes = sha256Hash.ComputeHash(bytes);
            return Encoding.UTF8.GetString(hashedBytes);
        }
    }
}