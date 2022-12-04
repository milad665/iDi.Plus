using System;
using iDi.Blockchain.Framework.Entities;

namespace iDi.Plus.Domain.Entities;

public class ConsentTx : EntityBase
{
    private ConsentTx()
    {}

    public ConsentTx(string consentTransactionHash, string holderPublicKey, byte[] doubleEncryptedData, DateTime consentTimeStamp, ConsentRequest request)
    {
        ConsentTransactionHash = consentTransactionHash;
        HolderPublicKey = holderPublicKey;
        DoubleEncryptedData = doubleEncryptedData;
        ConsentTimeStamp = consentTimeStamp;
        Request = request;
    }
    public string ConsentTransactionHash { get; set; }
    public string HolderPublicKey { get; set; }
    public byte[] DoubleEncryptedData { get; set; }
    public DateTime ConsentTimeStamp { get; set; }
    public ConsentRequest Request { get; set; }
}