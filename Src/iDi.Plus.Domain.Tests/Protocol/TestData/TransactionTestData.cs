using System;
using System.Text;
using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Plus.Domain.Tests.Protocol.TestData;

public class TransactionTestData
{
    public static TransactionTestData SampleTransactionIdCard2PassportName1 => new(new HashValue("1c667fd351a7ea72d99162fcf5628e7dd2d1dcdf98b0c33d417c45dd225a160e"), HashValue.Empty, CommonSampleData.IdCard1, CommonSampleData.IdCard2, null, "Passport", "Name", "DATA");
    public static TransactionTestData SampleTransactionIdCard2PassportExpirationDate1 => new(new HashValue("217482cf99166538e140294594d13dff712ba0195ccee94c3e176b6d2fd1f7d8"), HashValue.Empty, CommonSampleData.IdCard1, CommonSampleData.IdCard2, null, "Passport", "ExpirationDate", "DATA");
    public static TransactionTestData SampleTransactionIdCard3PassportName1 => new(new HashValue("74af99d6585331ea949090546442a9e6f4c5f9fc29291e78a18e4041b90f89e3"), HashValue.Empty, CommonSampleData.IdCard1, CommonSampleData.IdCard3, null, "Passport", "Name", "DATA");
    public static TransactionTestData SampleTransactionIdCard3PassportExpirationDate1 => new(new HashValue("162fe433afd216cde14a08a19a36fd84fd59e1debc6aca9d5d3fd3ccd806b345"), HashValue.Empty, CommonSampleData.IdCard1, CommonSampleData.IdCard3, null, "Passport", "ExpirationDate", "DATA");
    public static TransactionTestData SampleTransactionIdCard3DrivingLicenseName1 => new(new HashValue("5ecc367d04f081c981d59d82df52ae504c220ec62418df849dbac0d91bb97860"), HashValue.Empty, CommonSampleData.IdCard1, CommonSampleData.IdCard3, null, "DrivingLicense", "Name", "DATA");

    public static TransactionTestData SampleTransactionIdCard2PassportExpirationDate2 => new(new HashValue("b6c978677c0e835a090b8eaa18ba26e2602decded7052dd7185b3fa725d4e789"), SampleTransactionIdCard2PassportExpirationDate1.TransactionHash, CommonSampleData.IdCard1, CommonSampleData.IdCard2, null, "Passport", "ExpirationDate", "DATA");
    public static TransactionTestData SampleTransactionIdCard3PassportName2 => new(new HashValue("4ae9ed45da7968738f8ad4742611c333544a3a22afa4fb283a803c6efaa4b4dd"), SampleTransactionIdCard3PassportName1.TransactionHash, CommonSampleData.IdCard1, CommonSampleData.IdCard3, null, "Passport", "Name", "DATA");
    public static TransactionTestData SampleTransactionIdCard3PassportExpirationDate2 => new(new HashValue("bf027b86eac58ca5e81f9f8a4d1a5eee986807e11abe4d8f77c502890c47a8e2"), SampleTransactionIdCard3PassportExpirationDate1.TransactionHash, CommonSampleData.IdCard1, CommonSampleData.IdCard3, null, "Passport", "ExpirationDate", "DATA");

    public static TransactionTestData SampleInvalidTransactionIdCard3PassportExpirationDate3PreviousTransactionSubjectMismatch => new(new HashValue("683f2de3bb7b64fd6f859ca9f0f4f812f420f5c6884a9afc179371101ac62dfe"), SampleTransactionIdCard3DrivingLicenseName1.TransactionHash, CommonSampleData.IdCard1, CommonSampleData.IdCard3, null, "Passport", "ExpirationDate", "DATA");
    public static TransactionTestData SampleInvalidTransactionIdCard3PassportExpirationDate3PreviousTransactionIdentifierMismatch => new(new HashValue("723507d8358648a0f6fcaf35c0b3df1a60986a8f11da27270dc2a83f19e75e1d"), SampleTransactionIdCard3PassportName2.TransactionHash, CommonSampleData.IdCard1, CommonSampleData.IdCard3, null, "Passport", "ExpirationDate", "DATA");
    public static TransactionTestData SampleInvalidTransactionIdCard3PassportExpirationDate3PreviousTransactionHolderMismatch => new(new HashValue("ed2333efd969e0961cbbc34e1fa794cba0e8e2d0d134a64bc88adb6421e80f6a"), SampleTransactionIdCard2PassportExpirationDate2.TransactionHash, CommonSampleData.IdCard1, CommonSampleData.IdCard3, null, "Passport", "ExpirationDate", "DATA");
    
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