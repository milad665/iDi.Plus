using System;
using System.Text;
using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Plus.Domain.Tests.Protocol.TestData;

public class TransactionTestData
{
    public TransactionTestData(HashValue transactionHash, HashValue previousTransactionHash, IdCard issuer, IdCard holder, IdCard verifier, string subject, string identifier, string signedData)
    {
        TransactionHash = transactionHash;
        PreviousTransactionHash = previousTransactionHash;
        Issuer = issuer;
        Holder = holder;
        Verifier = verifier;
        Subject = subject;
        Identifier = identifier;
        SignedData = Encoding.UTF8.GetBytes(signedData);

        Timestamp = DateTime.UtcNow;
    }

    public HashValue TransactionHash { get; set; }
    public HashValue PreviousTransactionHash { get; set; }
    public IdCard Issuer { get; set; }
    public IdCard Holder { get; set; }
    public IdCard Verifier { get; set; }
    public string Subject { get; set; }
    public string Identifier { get; set; }
    public byte[] SignedData { get; set; }
    public DateTime Timestamp { get; set; }

}