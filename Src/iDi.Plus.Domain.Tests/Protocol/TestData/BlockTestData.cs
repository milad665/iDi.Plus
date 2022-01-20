using System;
using System.Collections.Generic;
using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Plus.Domain.Tests.Protocol.TestData;

public class BlockTestData
{
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