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
    public SampleDataProvider()
    {
        SampleLocalNodeKeys = DigitalSignatureKeys.Generate();

        SampleRemoteNodeKeys1 = DigitalSignatureKeys.Generate();
        SampleRemoteNodeKeys2 = DigitalSignatureKeys.Generate();
    }

    public DigitalSignatureKeys SampleLocalNodeKeys { get; }
    public DigitalSignatureKeys SampleRemoteNodeKeys1 { get; }
    public DigitalSignatureKeys SampleRemoteNodeKeys2 { get; }

    public byte[] BlockDataMessageBytes(bool produceValidPayloadSignature, Networks network, short version)
    {
        var senderNodeKeys = DigitalSignatureKeys.Generate();

        //Payload
        var payloadBytes = BlockDataPayloadBytes(BlockTestData.SampleBlock1);

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
        new IssueIdTransaction(CommonSampleData.IdCard1.Address, CommonSampleData.IdCard2.Address, "Passport", "Name", "DATA", null),
        new IssueIdTransaction(CommonSampleData.IdCard1.Address, CommonSampleData.IdCard2.Address, "Passport", "Photo", "DATA", null),
        new IssueIdTransaction(CommonSampleData.IdCard1.Address, CommonSampleData.IdCard2.Address, "Passport", "ExpirationDate", "DATA", null),
        new IssueIdTransaction(CommonSampleData.IdCard3.Address, CommonSampleData.IdCard2.Address, "DrivingLicense", "Type", "DATA", null),
        new IssueIdTransaction(CommonSampleData.IdCard3.Address, CommonSampleData.IdCard2.Address, "DrivingLicense", "Photo", "DATA", null),
        new IssueIdTransaction(CommonSampleData.IdCard3.Address, CommonSampleData.IdCard2.Address, "DrivingLicense", "ExpirationDate", "DATA", null),
        new IssueIdTransaction(CommonSampleData.IdCard3.Address, CommonSampleData.IdCard2.Address, "DrivingLicense", "Tickets", "DATA", null),
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
    public byte[] CreateTxPayloadBytes(TransactionTestData transactionTestData)
    {
        var txDataPayloadBytes = TxDataPayloadBytes(transactionTestData);
        var cryptoServiceProvider = new CryptoServiceProvider();
        var encrypted =
            cryptoServiceProvider.EncryptByPrivateKey(txDataPayloadBytes, transactionTestData.Issuer.PrivateKey);
        var lstBytes = new List<byte>();
        lstBytes.AddRange(transactionTestData.Issuer.PublicKey);
        lstBytes.AddRange(encrypted);
        return lstBytes.ToArray();
    }

    public byte[] GetBlockPayloadBytes(HashValue blockHash)
    {
        return blockHash.Bytes;
    }
    public byte[] GetNewBlocksPayloadBytes(long lastBlockIndex)
    {
        return BitConverter.GetBytes(lastBlockIndex);
    }
    public byte[] GetTxPayloadBytes(HashValue transactionHash)
    {
        return transactionHash.Bytes;
    }
    public byte[] NewBlocksPayloadBytes(List<HashValue> blockHashes)
    {
        var bytes = new List<byte>();
        foreach (var hash in blockHashes)
            bytes.AddRange(hash.Bytes);

        return bytes.ToArray();
    }
    public byte[] NewTxsPayloadBytes(List<HashValue> transactionHashes)
    {
        var bytes = new List<byte>();
        foreach (var hash in transactionHashes)
            bytes.AddRange(hash.Bytes);

        return bytes.ToArray();
    }
    public byte[] TxDataPayloadBytes(TransactionTestData transactionTestData)
    {
        var lstBytes = new List<byte>();
        lstBytes.AddRange(transactionTestData.TransactionHash.Bytes);
        lstBytes.Add((byte)TransactionTypes.IssueTransaction);
        lstBytes.AddRange(transactionTestData.Issuer.Address.HexStringToByteArray());
        lstBytes.AddRange(transactionTestData.Holder.Address.HexStringToByteArray());
        lstBytes.AddRange(transactionTestData.Verifier?.Address?.HexStringToByteArray() ?? new byte[IdCard.PublicKeyByteLength]);

        var subjectPadded = transactionTestData.Subject.PadRight(FrameworkEnvironment.SubjectByteLength);
        var identifierKeyPadded = transactionTestData.Identifier.PadRight(FrameworkEnvironment.IdentifierByteLength);
        lstBytes.AddRange(Encoding.ASCII.GetBytes(subjectPadded));
        lstBytes.AddRange(Encoding.ASCII.GetBytes(identifierKeyPadded));
        lstBytes.AddRange(BitConverter.GetBytes(transactionTestData.Timestamp.Ticks));
        lstBytes.AddRange(transactionTestData.PreviousTransactionHash?.Bytes ?? new byte[HashValue.HashByteLength]);
        lstBytes.AddRange(transactionTestData.SignedData);

        return lstBytes.ToArray();
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



    public Message BlockDataMessage(BlockTestData blockTestData)
    {
        var payload = new BlockDataPayload(BlockDataPayloadBytes(blockTestData));
        var header = CreateHeader(MessageTypes.BlockData, payload, SampleRemoteNodeKeys1);
        return Message.Create(header, payload);
    }
    public Message CreateTxMessage(TransactionTestData transactionTestData)
    {
        var payload = new CreateTxPayload(CreateTxPayloadBytes(transactionTestData));
        var header = CreateHeader(MessageTypes.CreateTx, payload, SampleRemoteNodeKeys1);
        return Message.Create(header, payload);
    }
    public Message GetBlockMessage(HashValue blockHash)
    {
        var payload = new GetBlockPayload(GetBlockPayloadBytes(blockHash));
        var header = CreateHeader(MessageTypes.GetBlock, payload, SampleRemoteNodeKeys1);
        return Message.Create(header, payload);
    }
    public Message GetNewBlocksMessage(long lastBlockIndex)
    {
        var payload = new GetNewBlocksPayload(GetNewBlocksPayloadBytes(lastBlockIndex));
        var header = CreateHeader(MessageTypes.GetNewBlocks, payload, SampleRemoteNodeKeys1);
        return Message.Create(header, payload);
    }
    public Message GetTxMessage(HashValue transactionHash)
    {
        var payload = new GetTxPayload(GetTxPayloadBytes(transactionHash));
        var header = CreateHeader(MessageTypes.GetTx, payload, SampleRemoteNodeKeys1);
        return Message.Create(header, payload);
    }
    public Message NewBlocksMessage(List<HashValue> blockHashes)
    {
        var payload = new NewBlocksPayload(NewBlocksPayloadBytes(blockHashes));
        var header = CreateHeader(MessageTypes.NewBlocks, payload, SampleRemoteNodeKeys1);
        return Message.Create(header, payload);
    }
    public Message NewTxsMessage(List<HashValue> transactionHashes)
    {
        var payload = new NewTxsPayload(NewTxsPayloadBytes(transactionHashes));
        var header = CreateHeader(MessageTypes.NewTxs, payload, SampleRemoteNodeKeys1);
        return Message.Create(header, payload);
    }
    public Message TxDataMessage(TransactionTestData transactionTestData)
    {
        var payload = new TxDataPayload(TxDataPayloadBytes(transactionTestData));
        var header = CreateHeader(MessageTypes.TxData, payload, SampleRemoteNodeKeys1);
        return Message.Create(header, payload);
    }


    private Header CreateHeader(MessageTypes messageType, IPayload payload, KeyPair localKeys)
    {
        var signature = SignPayload(payload, localKeys);
        return Header.Create(Networks.Main, 1, SampleRemoteNodeKeys1.PublicKey.ToHexString(), messageType,
            payload.RawData.Length, signature);
    }
    private byte[] SignPayload(IPayload payload, KeyPair localKeys)
    {
        var cryptoService = new CryptoServiceProvider();
        var signature = cryptoService.Sign(localKeys.PrivateKey, payload?.RawData);
        return signature;
    }
}