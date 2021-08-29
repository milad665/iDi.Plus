namespace iDi.Blockchain.Core
{
    public interface ITransaction
    {
        string TransactionHash { get;}

        void Verify();
        string ComputeHash();
    }
}