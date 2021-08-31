namespace iDi.Blockchain.Core
{
    public interface ITransaction
    {
        string TransactionHash { get;}
        TransactionTypes TransactionType { get;  }

        void Verify();
        string ComputeHash();
    }
}