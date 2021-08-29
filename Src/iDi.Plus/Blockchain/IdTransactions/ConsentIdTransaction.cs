using System.Security.Cryptography;
using System.Text;

namespace iDi.Plus.Domain.Blockchain.IdTransactions
{
    public sealed class ConsentIdTransaction : IdTransaction
    {
        private ConsentIdTransaction()
        {}

        public ConsentIdTransaction(string issuerPublicKey, string holderPublicKey, string identifierKey, string signedData, string verifierPublicKey, string previousTransactionHash) 
            : base(TransactionTypes.ConsentTransaction,issuerPublicKey, holderPublicKey, identifierKey, signedData, previousTransactionHash)
        {
            VerifierPublicKey = verifierPublicKey;

            TransactionHash = ComputeHash();
        }

        public string VerifierPublicKey { get; set; }
        public override string ComputeHash()
        {
            var tx =
                $"{TransactionType}:{IssuerPublicKey}:{HolderPublicKey}:{VerifierPublicKey}:{IdentifierKey}:{Timestamp:yyMMddHHmmss.FFFFFFF}:{SignedData}";
            var bytes = Encoding.UTF8.GetBytes(tx);
            using var sha256Hash = SHA256.Create();
            var hashedBytes = sha256Hash.ComputeHash(bytes);
            return Encoding.UTF8.GetString(hashedBytes);
        }
    }
}