using System;
using System.Collections.Generic;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

/// <summary>
/// Payload of CreateTx (Create Transaction) command
/// </summary>
public class CreateTxPayload : MainNetworkV1PayloadBase
{
    public CreateTxPayload(byte[] rawData) : base(rawData, MessageTypes.CreateTx)
    {
        ExtractData(rawData);
    }

    private CreateTxPayload(byte[] senderPublicKey, byte[] encryptedTransactionData, byte[] rawData) : base(rawData, MessageTypes.CreateTx)
    {
        SenderPublicKey = senderPublicKey;
        EncryptedTransactionData = encryptedTransactionData;
        //TxDataPayload = txDataPayload;
    }

    public static CreateTxPayload FromTxDataPayload(IdCard senderKeys, TxDataResponsePayload txDataResponse)
    {
        var csp = new CryptoServiceProvider();
        var encryptedTransactionData = csp.EncryptByPrivateKey(txDataResponse.RawData, senderKeys.PrivateKey);

        var bytes = new List<byte>();
        bytes.AddRange(senderKeys.PublicKey);
        bytes.AddRange(encryptedTransactionData);

        return new CreateTxPayload(senderKeys.PublicKey, encryptedTransactionData, bytes.ToArray());
    }

    public byte[] SenderPublicKey { get; private set; }
    public byte[] EncryptedTransactionData { get; private set; }

    private void ExtractData(byte[] rawData)
    {
        var span = new ReadOnlySpan<byte>(rawData);
        var index = 0;
        var senderPublicKey = span.Slice(index, IdCard.PublicKeyLength).ToArray();
        index += IdCard.PublicKeyLength;
        var encryptedTransactionData = span.Slice(index).ToArray();

        SenderPublicKey = senderPublicKey;
        EncryptedTransactionData = encryptedTransactionData;
    }
}