using System.Security.Cryptography;
using System.Text;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Exceptions;

namespace iDi.Blockchain.Framework.Tests;

public class TestTransaction : ITransaction
{
    public TestTransaction(TransactionTypes transactionType, string someValue)
    {
        TransactionType = transactionType;
        SomeValue = someValue;
        TransactionHash = ComputeHash();
    }

    public string TransactionHash { get; private set; }
    public TransactionTypes TransactionType { get; }
    public string SomeValue { get; }

    public void Verify()
    {
        if (TransactionHash != ComputeHash())
            throw new HashMismatchIdPlusException("Invalid transaction hash.");
    }

    public string ComputeHash()
    {
        var tx =
            $"{TransactionType}:{SomeValue}";
        var bytes = Encoding.UTF8.GetBytes(tx);
        using var sha256Hash = SHA256.Create();
        var hashedBytes = sha256Hash.ComputeHash(bytes);
        return Encoding.UTF8.GetString(hashedBytes);
    }
}