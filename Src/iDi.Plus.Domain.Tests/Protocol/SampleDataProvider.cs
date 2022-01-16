using System;
using System.Collections.Generic;
using System.Text;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Extensions;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Tests.Protocol;

public class SampleDataProvider
{
    private readonly IdCard IdCard1;
    private readonly IdCard IdCard2;
    private readonly IdCard IdCard3;
    private readonly IdCard IdCard4;

    public SampleDataProvider()
    {
        IdCard1 = IdCard.Generate();
        IdCard2 = IdCard.Generate();
        IdCard3 = IdCard.Generate();
        IdCard4 = IdCard.Generate();
    }

    public byte[] BlockDataMessageBytes(bool produceValidPayloadSignature, Networks network, short version)
    {
        var senderNodeKeys = DigitalSignatureKeys.Generate();

        //Payload
        var payloadBytes = new List<byte>();
        payloadBytes.AddRange(BitConverter.GetBytes((long)0)); //Index
        payloadBytes.AddRange("5b99a041116f5e12057525d153704123b30d3003f806bfb8b793d93e3cdd99fc".HexStringToByteArray());
        payloadBytes.AddRange("3b083c71dd98ce54c7608b36471067a5acbf0ed447fce5aba705bf943cf56c83".HexStringToByteArray());
        payloadBytes.AddRange(BitConverter.GetBytes(DateTime.UtcNow.Ticks));
        payloadBytes.AddRange(BitConverter.GetBytes((long)1)); //Nonce
        foreach (var txData in GetSampleTxDataPayloadBytes())
        {
            payloadBytes.AddRange(BitConverter.GetBytes(txData.Length));
            payloadBytes.AddRange(txData);
        }
        //Last 4 Bytes of this payload must be zeros
        payloadBytes.AddRange(BitConverter.GetBytes((int)0));

        //Header
        var networkBytes = BitConverter.GetBytes((int)network);
        var versionBytes = BitConverter.GetBytes(version);
        var nodeIdBytes = senderNodeKeys.PublicKey;
        var messageTypeBytes = BitConverter.GetBytes((byte)MessageTypes.BlockData)[0];
        var payloadLengthBytes = BitConverter.GetBytes(payloadBytes.Count);

        var cryptoServiceProvider = new CryptoServiceProvider();
        var payloadSignature = cryptoServiceProvider.Sign(senderNodeKeys.PrivateKey, payloadBytes.ToArray());

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

    public List<byte[]> GetSampleTxDataPayloadBytes() => new()
    {
        CreateSampleTransactionData("1c667fd351a7ea72d99162fcf5628e7dd2d1dcdf98b0c33d417c45dd225a160e",IdCard1.Address, IdCard2.Address, "Passport", "Name", "217482cf99166538e140294594d13dff712ba0195ccee94c3e176b6d2fd1f7d8"),
        CreateSampleTransactionData("217482cf99166538e140294594d13dff712ba0195ccee94c3e176b6d2fd1f7d8", IdCard1.Address, IdCard2.Address, "Passport", "ExpirationDate", "217482cf99166538e140294594d13dff712ba0195ccee94c3e176b6d2fd1f7d8"),
        CreateSampleTransactionData("74af99d6585331ea949090546442a9e6f4c5f9fc29291e78a18e4041b90f89e3", IdCard1.Address, IdCard3.Address, "Passport", "Name", "217482cf99166538e140294594d13dff712ba0195ccee94c3e176b6d2fd1f7d8"),
        CreateSampleTransactionData("162fe433afd216cde14a08a19a36fd84fd59e1debc6aca9d5d3fd3ccd806b345", IdCard1.Address, IdCard3.Address, "Passport", "ExpirationDate", "217482cf99166538e140294594d13dff712ba0195ccee94c3e176b6d2fd1f7d8")
    };

    public byte[] CreateSampleTransactionData(string transactionHash, string issuerAddress, string holderAddress, string subject, string identifierKey, string previousTransactionHash)
    {
        var lstBytes = new List<byte>();
        lstBytes.AddRange(transactionHash.HexStringToByteArray());
        lstBytes.Add((byte)TransactionTypes.IssueTransaction);
        lstBytes.AddRange(issuerAddress.HexStringToByteArray());
        lstBytes.AddRange(holderAddress.HexStringToByteArray());
        lstBytes.AddRange(new byte[IdCard.PublicKeyByteLength]);//Verifier = Empty

        var subjectPadded = subject.PadRight(TxDataPayload.SubjectByteLength);
        var identifierKeyPadded = identifierKey.PadRight(TxDataPayload.IdentifierByteLength);
        lstBytes.AddRange(Encoding.ASCII.GetBytes(subjectPadded));
        lstBytes.AddRange(Encoding.ASCII.GetBytes(identifierKeyPadded));
        lstBytes.AddRange(BitConverter.GetBytes(DateTime.UtcNow.Ticks));
        lstBytes.AddRange(string.IsNullOrWhiteSpace(previousTransactionHash) ? new byte[FrameworkEnvironment.HashAlgorithm.HashSize / 8] : previousTransactionHash.HexStringToByteArray());
        var random = new Random();
        var signature = new byte[100];
        random.NextBytes(signature);
        lstBytes.AddRange(signature);

        return lstBytes.ToArray();
    }
}