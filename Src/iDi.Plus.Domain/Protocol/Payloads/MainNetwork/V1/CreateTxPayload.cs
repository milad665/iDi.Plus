using System;
using System.Collections.Generic;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Exceptions;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

/// <summary>
/// Payload of CreateTx (Create Transaction) command
/// </summary>
public class CreateTxPayload : MainNetworkV1PayloadBase
{
    public CreateTxPayload(byte[] rawData) : base(rawData, MessageTypes.CreateTx)
    {
        ExtractAndVerifyData(rawData);
    }

    private CreateTxPayload(byte[] senderPublicKey, byte[] encryptedTransactionData, TxDataPayload txDataPayload, byte[] rawData) : base(rawData, MessageTypes.CreateTx)
    {
        SenderPublicKey = senderPublicKey;
        EncryptedTransactionData = encryptedTransactionData;
        TxDataPayload = txDataPayload;
    }

    public static CreateTxPayload FromTxDataPayload(IdCard senderKeys, TxDataPayload txData)
    {
        var csp = new CryptoServiceProvider();
        var encryptedTransactionData = csp.EncryptByPrivateKey(txData.RawData, senderKeys.PrivateKey);

        var bytes = new List<byte>();
        bytes.AddRange(senderKeys.PublicKey);
        bytes.AddRange(encryptedTransactionData);

        return new CreateTxPayload(senderKeys.PublicKey, encryptedTransactionData, txData, bytes.ToArray());
    }

    public byte[] SenderPublicKey { get; private set; }
    public byte[] EncryptedTransactionData { get; private set; }
    public TxDataPayload TxDataPayload { get; private set; }

    private void ExtractAndVerifyData(byte[] rawData)
    {
        var span = new ReadOnlySpan<byte>(rawData);
        var index = 0;
        var senderPublicKey = span.Slice(index, IdCard.PublicKeyByteLength).ToArray();
        index += IdCard.PublicKeyByteLength;
        var encryptedTransactionData = span.Slice(index).ToArray();

        var csp = new CryptoServiceProvider();
        var decryptedData = csp.DecryptByPublicKey(encryptedTransactionData, senderPublicKey);
        var txDataPayload = new TxDataPayload(decryptedData);
        var senderAddress = new AddressValue(senderPublicKey);

        switch (txDataPayload.TransactionType)
        {
            case TransactionTypes.IssueTransaction when txDataPayload.IssuerAddress.Equals(senderAddress):
                break;
            case TransactionTypes.ConsentTransaction when txDataPayload.HolderAddress.Equals(senderAddress):
                break;
            default:
                throw new UnauthorizedException("Cannot verify transaction sender.");
        }

        SenderPublicKey = senderPublicKey;
        EncryptedTransactionData = encryptedTransactionData;
        TxDataPayload = txDataPayload;
    }
}