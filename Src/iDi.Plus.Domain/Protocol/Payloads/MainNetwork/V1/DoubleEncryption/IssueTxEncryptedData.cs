using System;
using System.Text;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Extensions;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1.DoubleEncryption;

public class IssueTxEncryptedData
{
    private IssueTxEncryptedData(DateTime issueTransactionTimestamp, string issuerPublicKey, string holderPublicKey, string subject, string identifierKey, string value, DateTime expiresOn)
    {
        IssueTransactionTimestamp = issueTransactionTimestamp;
        IssuerPublicKey = issuerPublicKey;
        HolderPublicKey = holderPublicKey;
        Subject = subject;
        IdentifierKey = identifierKey;
        Value = value;
        ExpiresOn = expiresOn;
    }

    public static IssueTxEncryptedData FromIssueCipherData(byte[] data, byte[] issuerPublicKey)
    {
        var csp = new CryptoServiceProvider();
        var rawData = csp.DecryptByPublicKey(data, issuerPublicKey);
        return ExtractData(rawData);
    }

    public DateTime IssueTransactionTimestamp { get; }
    public string IssuerPublicKey { get; }
    public string HolderPublicKey { get; }
    public string Subject { get; }
    public string IdentifierKey { get; }
    public DateTime ExpiresOn { get; }
    public string Value { get; }

    private static IssueTxEncryptedData ExtractData(byte[] rawData)
    {
        var span = new ReadOnlySpan<byte>(rawData);
        var index = 0;
        var issueTxTimestamp = DateTime.FromBinary(BitConverter.ToInt64(span.Slice(index, 8)));
        index += 8;
        var issuerAddress = span.Slice(index, IdCard.PublicKeyByteLength).ToHexString();
        index += IdCard.PublicKeyByteLength;
        var holderAddress = span.Slice(index, IdCard.PublicKeyByteLength).ToHexString();
        index += IdCard.PublicKeyByteLength;

        var subject = Encoding.ASCII.GetString(span.Slice(index, FrameworkEnvironment.SubjectByteLength)).Trim();
        index += FrameworkEnvironment.SubjectByteLength;
        var identifierKey = Encoding.ASCII.GetString(span.Slice(index, FrameworkEnvironment.IdentifierByteLength)).Trim();
        index += FrameworkEnvironment.IdentifierByteLength;
        var expiresOn = DateTime.FromBinary(BitConverter.ToInt64(span.Slice(index, 8)));
        index += 8;
        var value = Encoding.ASCII.GetString(span.Slice(index));

        return new IssueTxEncryptedData(issueTxTimestamp, issuerAddress, holderAddress, subject, identifierKey, value,
            expiresOn);
    }
}