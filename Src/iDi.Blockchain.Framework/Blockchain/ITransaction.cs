using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Blockchain.Framework.Blockchain
{
    public interface ITransaction
    {
        HashValue TransactionHash { get;}
        TransactionTypes TransactionType { get;  }

        void VerifyHash();
    }
}