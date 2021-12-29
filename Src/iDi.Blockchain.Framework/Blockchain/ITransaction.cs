namespace iDi.Blockchain.Framework.Blockchain
{
    public interface ITransaction
    {
        string TransactionHash { get;}
        TransactionTypes TransactionType { get;  }

        void Verify();
        string ComputeHash();
    }
}