using System;
using System.Collections.Generic;
using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Plus.Domain.Tests.Protocol.TestData;

public class BlockTestData
{
    public static BlockTestData SampleBlock1 = new BlockTestData(0,
        new HashValue("5b99a041116f5e12057525d153704123b30d3003f806bfb8b793d93e3cdd99fc"),
        new HashValue("3b083c71dd98ce54c7608b36471067a5acbf0ed447fce5aba705bf943cf56c83"), DateTime.UtcNow, 1,
        new List<TransactionTestData>
        {
            TransactionTestData.SampleTransactionIdCard2PassportName1,
            TransactionTestData.SampleTransactionIdCard2PassportExpirationDate1,
            TransactionTestData.SampleTransactionIdCard3PassportName1,
            TransactionTestData.SampleTransactionIdCard3PassportExpirationDate1
        });
    public static BlockTestData SampleBlock2 = new BlockTestData(0,
        new HashValue("1819dcfe58fc5af2290c734dcdf4d61fe2306b680df0f691abeb5eb7df6352a8"),
        new HashValue("5b99a041116f5e12057525d153704123b30d3003f806bfb8b793d93e3cdd99fc"), DateTime.UtcNow, 1,
        new List<TransactionTestData>
        {
            TransactionTestData.SampleTransactionIdCard2PassportExpirationDate2,
            TransactionTestData.SampleTransactionIdCard3PassportExpirationDate2
        });
    public static BlockTestData SampleBlock3 = new BlockTestData(0,
        new HashValue("c33dc0177c447aa5f1ac9f9b4191c1131d21c4955d700bcc9918ec97a741570e"),
        new HashValue("1819dcfe58fc5af2290c734dcdf4d61fe2306b680df0f691abeb5eb7df6352a8"), DateTime.UtcNow, 1,
        new List<TransactionTestData>
        {
            TransactionTestData.SampleTransactionIdCard3DrivingLicenseName1,
        });
    public static BlockTestData SampleBlock4 = new BlockTestData(0,
        new HashValue("3d5f5c5362f522ea57c5b1a08d06e815d4bf921c034642cb4e31fd58438291a3"),
        new HashValue("c33dc0177c447aa5f1ac9f9b4191c1131d21c4955d700bcc9918ec97a741570e"), DateTime.UtcNow, 1,
        new List<TransactionTestData>
        {
            TransactionTestData.SampleInvalidTransactionIdCard3PassportExpirationDate3PreviousTransactionHolderMismatch,
            TransactionTestData.SampleInvalidTransactionIdCard3PassportExpirationDate3PreviousTransactionIdentifierMismatch,
            TransactionTestData.SampleInvalidTransactionIdCard3PassportExpirationDate3PreviousTransactionSubjectMismatch,
        });

    public BlockTestData(long index, HashValue hash, HashValue previousHash, DateTime timeStamp, long nonce, List<TransactionTestData> transactions)
    {
        Index = index;
        Hash = hash;
        PreviousHash = previousHash;
        TimeStamp = timeStamp;
        Nonce = nonce;
        Transactions = transactions;
    }

    public long Index { get; set; }
    public HashValue Hash { get; set; }
    public HashValue PreviousHash { get; set; }
    public DateTime TimeStamp { get; set; }
    public long Nonce { get; set; }
    public List<TransactionTestData> Transactions { get; set; }

}