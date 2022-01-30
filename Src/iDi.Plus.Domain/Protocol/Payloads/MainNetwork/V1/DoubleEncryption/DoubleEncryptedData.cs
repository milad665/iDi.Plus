using System;
using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1.DoubleEncryption;

public class DoubleEncryptedData
{
    protected DoubleEncryptedData(byte[] issuerPublicKey, IssueTxEncryptedData issuedIdentifier, byte[] issueTxEncryptedRawData)
    {
        IssuerPublicKey = issuerPublicKey;
        IssuedIdentifier = issuedIdentifier;
        IssueTxEncryptedRawData = issueTxEncryptedRawData;
    }

    public static DoubleEncryptedData FromDataEncryptedByPublicKey(byte[] data, byte[] holderOrVerifierPrivateKey)
    {
        var csp = new CryptoServiceProvider();
        var rawData = csp.DecryptByPublicKey(data, holderOrVerifierPrivateKey);
        return ExtractData(rawData);
    }

    public byte[] IssuerPublicKey { get; }
    public IssueTxEncryptedData IssuedIdentifier { get; }
    public byte[] IssueTxEncryptedRawData { get; }

    protected static DoubleEncryptedData ExtractData(byte[] rawData)
    {
        var span = new ReadOnlySpan<byte>(rawData);
        var index = 0;
        var issuerPublicKey = span.Slice(index, IdCard.PublicKeyByteLength).ToArray();
        index += IdCard.PublicKeyByteLength;

        var issueTxRawData = span.Slice(index).ToArray();

        var issueTxData = IssueTxEncryptedData.FromIssueCipherData(issueTxRawData, issuerPublicKey);
        return new DoubleEncryptedData(issuerPublicKey, issueTxData, issueTxRawData);
    }
}