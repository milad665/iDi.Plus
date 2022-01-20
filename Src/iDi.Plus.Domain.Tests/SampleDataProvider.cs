using System;
using System.Collections.Generic;
using System.Text;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;
using iDi.Plus.Domain.Tests.Protocol.TestData;

namespace iDi.Plus.Domain.Tests;

public class SampleDataProvider
{
    public IdCard IdCard1 { get; }
    public IdCard IdCard2 { get; }
    public IdCard IdCard3 { get; }

    public TransactionTestData Transaction1 => new TransactionTestData(new HashValue("1c667fd351a7ea72d99162fcf5628e7dd2d1dcdf98b0c33d417c45dd225a160e"), new HashValue("217482cf99166538e140294594d13dff712ba0195ccee94c3e176b6d2fd1f7d8"), IdCard1, IdCard2, null, "Passport", "Name", "DATA");
    public TransactionTestData Transaction2 => new TransactionTestData(new HashValue("217482cf99166538e140294594d13dff712ba0195ccee94c3e176b6d2fd1f7d8"), new HashValue("217482cf99166538e140294594d13dff712ba0195ccee94c3e176b6d2fd1f7d8"), IdCard1, IdCard2, null, "Passport", "ExpirationDate", "DATA");
    public TransactionTestData Transaction3 => new TransactionTestData(new HashValue("74af99d6585331ea949090546442a9e6f4c5f9fc29291e78a18e4041b90f89e3"), new HashValue("217482cf99166538e140294594d13dff712ba0195ccee94c3e176b6d2fd1f7d8"), IdCard1, IdCard3, null, "Passport", "Name", "DATA");
    public TransactionTestData Transaction4 => new TransactionTestData(new HashValue("162fe433afd216cde14a08a19a36fd84fd59e1debc6aca9d5d3fd3ccd806b345"), new HashValue("217482cf99166538e140294594d13dff712ba0195ccee94c3e176b6d2fd1f7d8"), IdCard1, IdCard3, null, "Passport", "ExpirationDate", "DATA");

    public BlockTestData Block1 => new BlockTestData(0,
        new HashValue("5b99a041116f5e12057525d153704123b30d3003f806bfb8b793d93e3cdd99fc"),
        new HashValue("3b083c71dd98ce54c7608b36471067a5acbf0ed447fce5aba705bf943cf56c83"), DateTime.UtcNow, 1,
        new List<TransactionTestData>
        {
            Transaction1,
            Transaction2,
            Transaction3,
            Transaction4
        });

    public SampleDataProvider()
    {
        IdCard1 = IdCard.Generate();
        IdCard2 = IdCard.Generate();
        IdCard3 = IdCard.Generate();
    }

    public byte[] BlockDataMessageBytes(bool produceValidPayloadSignature, Networks network, short version)
    {
        var senderNodeKeys = DigitalSignatureKeys.Generate();

        //Payload
        var payloadBytes = BlockDataPayloadBytes(Block1);

        //Header
        var networkBytes = BitConverter.GetBytes((int)network);
        var versionBytes = BitConverter.GetBytes(version);
        var nodeIdBytes = senderNodeKeys.PublicKey;
        var messageTypeBytes = BitConverter.GetBytes((byte)MessageTypes.BlockData)[0];
        var payloadLengthBytes = BitConverter.GetBytes(payloadBytes.Length);

        var cryptoServiceProvider = new CryptoServiceProvider();
        var payloadSignature = cryptoServiceProvider.Sign(senderNodeKeys.PrivateKey, payloadBytes);

        if (!produceValidPayloadSignature)
        {
            var random = new Random();
            random.NextBytes(payloadSignature);
        }

        var headerBytes = new List<byte>();
        headerBytes.AddRange(networkBytes); //4
        headerBytes.AddRange(versionBytes); //2
        headerBytes.AddRange(nodeIdBytes);
        headerBytes.Add(messageTypeBytes);
        headerBytes.AddRange(payloadLengthBytes);
        headerBytes.AddRange(payloadSignature);
        var headerLengthBytes = BitConverter.GetBytes(headerBytes.Count);
        var result = new List<byte>();
        result.AddRange(headerLengthBytes);
        result.AddRange(headerBytes);
        result.AddRange(payloadBytes);

        return result.ToArray();
    }
    public List<IdTransaction> GetSampleIdTransactions() => new()
    {
        new IssueIdTransaction(IdCard1.Address, IdCard2.Address, "Passport", "Name", "DATA", null),
        new IssueIdTransaction(IdCard1.Address, IdCard2.Address, "Passport", "Photo", "DATA", null),
        new IssueIdTransaction(IdCard1.Address, IdCard2.Address, "Passport", "ExpirationDate", "DATA", null),
        new IssueIdTransaction(IdCard3.Address, IdCard2.Address, "DrivingLicense", "Type", "DATA", null),
        new IssueIdTransaction(IdCard3.Address, IdCard2.Address, "DrivingLicense", "Photo", "DATA", null),
        new IssueIdTransaction(IdCard3.Address, IdCard2.Address, "DrivingLicense", "ExpirationDate", "DATA", null),
        new IssueIdTransaction(IdCard3.Address, IdCard2.Address, "DrivingLicense", "Tickets", "DATA", null),
    };
    public byte[] BlockDataPayloadBytes(BlockTestData blockTestData)
    {
        var payloadBytes = new List<byte>();
        payloadBytes.AddRange(BitConverter.GetBytes(blockTestData.Index));
        payloadBytes.AddRange(blockTestData.Hash.Bytes);
        payloadBytes.AddRange(blockTestData.PreviousHash.Bytes);
        payloadBytes.AddRange(BitConverter.GetBytes(blockTestData.TimeStamp.Ticks));
        payloadBytes.AddRange(BitConverter.GetBytes(blockTestData.Nonce));
        if (blockTestData.Transactions != null)
        {
            foreach (var tx in blockTestData.Transactions)
            {
                var txData = TxDataPayloadBytes(tx);
                payloadBytes.AddRange(BitConverter.GetBytes(txData.Length));
                payloadBytes.AddRange(txData);
            }
        }

        //Last 4 Bytes of this payload must be zeros
        payloadBytes.AddRange(BitConverter.GetBytes((int)0));
        return payloadBytes.ToArray();
    }
    public byte[] CreateTxPayloadBytes(string transactionHash, byte[] signedData, string issuerPublicKey)
    {
        var lstBytes = new List<byte>();
        lstBytes.AddRange(transactionHash.HexStringToByteArray());
        lstBytes.AddRange(BitConverter.GetBytes(signedData.Length));
        lstBytes.AddRange(signedData);
        lstBytes.AddRange(issuerPublicKey.HexStringToByteArray());
        return lstBytes.ToArray();
    }
    public byte[] TxDataPayloadBytes(TransactionTestData transactionTestData)
    {
        var lstBytes = new List<byte>();
        lstBytes.AddRange(transactionTestData.TransactionHash.Bytes);
        lstBytes.Add((byte)TransactionTypes.IssueTransaction);
        lstBytes.AddRange(transactionTestData.Issuer.Address.HexStringToByteArray());
        lstBytes.AddRange(transactionTestData.Holder.Address.HexStringToByteArray());
        lstBytes.AddRange(transactionTestData.Verifier?.Address?.HexStringToByteArray() ?? new byte[IdCard.PublicKeyByteLength]);

        var subjectPadded = transactionTestData.Subject.PadRight(TxDataPayload.SubjectByteLength);
        var identifierKeyPadded = transactionTestData.Identifier.PadRight(TxDataPayload.IdentifierByteLength);
        lstBytes.AddRange(Encoding.ASCII.GetBytes(subjectPadded));
        lstBytes.AddRange(Encoding.ASCII.GetBytes(identifierKeyPadded));
        lstBytes.AddRange(BitConverter.GetBytes(transactionTestData.Timestamp.Ticks));
        lstBytes.AddRange(transactionTestData.PreviousTransactionHash?.Bytes ?? new byte[HashValue.HashByteLength]);
        lstBytes.AddRange(transactionTestData.SignedData);

        return lstBytes.ToArray();
    }
}