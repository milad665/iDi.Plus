namespace iDi.Blockchain.Framework
{
    public interface ITransaction
    {
        string TransactionHash { get;}
        TransactionTypes TransactionType { get;  }

        void Verify();
        string ComputeHash();
    }
}