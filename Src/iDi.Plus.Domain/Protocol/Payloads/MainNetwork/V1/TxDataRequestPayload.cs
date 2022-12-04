using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

/// <summary>
/// Decrypted transaction data of a CreateTx request
/// </summary>
public class TxDataRequestPayload : MainNetworkV1PayloadBase
{
    protected const int MimeTypeLength = 100;
    public TxDataRequestPayload(byte[] rawData):base(rawData, MessageTypes.TxDataRequest)
    {
        ExtractData(rawData);
    }

    private TxDataRequestPayload(HashValue transactionHash, TransactionTypes transactionType, AddressValue issuerAddress, AddressValue holderAddress, AddressValue verifierAddress, string subject, string identifierKey, string valueMimeType, byte[] value, byte[] rawData) : base(rawData, MessageTypes.TxDataRequest)
    {
        TransactionHash = transactionHash;
        TransactionType = transactionType;
        IssuerAddress = issuerAddress;
        HolderAddress = holderAddress;
        VerifierAddress = verifierAddress;
        Subject = subject;
        IdentifierKey = identifierKey;
        Value = value;
        ValueMimeType = valueMimeType;
    }

    public new static TxDataRequestPayload Create(HashValue transactionHash, TransactionTypes transactionType, AddressValue issuerAddress, AddressValue holderAddress, AddressValue verifierAddress, string subject, string identifierKey, string mimeType, byte[] value)
    {
        if (subject.Length > FrameworkEnvironment.SubjectByteLength)
            throw new InvalidInputException($"Subject length can not be over {FrameworkEnvironment.SubjectByteLength} characters");

        if (identifierKey.Length > FrameworkEnvironment.IdentifierByteLength)
            throw new InvalidInputException($"IdentifierKey length can not be over {FrameworkEnvironment.IdentifierByteLength} characters");

        var lstBytes = new List<byte>();
        lstBytes.AddRange(transactionHash.Bytes);
        lstBytes.Add((byte)transactionType);
        lstBytes.AddRange(issuerAddress.Bytes);
        lstBytes.AddRange(holderAddress.Bytes);
        if (verifierAddress == null)
            verifierAddress = AddressValue.Empty;
        lstBytes.AddRange(verifierAddress.Bytes);

        var subjectPadded = subject.PadRight(FrameworkEnvironment.SubjectByteLength);
        var identifierKeyPadded = identifierKey.PadRight(FrameworkEnvironment.IdentifierByteLength);
        lstBytes.AddRange(Encoding.ASCII.GetBytes(subjectPadded));
        lstBytes.AddRange(Encoding.ASCII.GetBytes(identifierKeyPadded));
        lstBytes.AddRange(StringToByteArray(mimeType, MimeTypeLength));
        lstBytes.AddRange(value);

        return new TxDataRequestPayload(transactionHash, transactionType, issuerAddress, holderAddress, verifierAddress,
            subject, identifierKey, mimeType, value, lstBytes.ToArray());
    }

    public HashValue TransactionHash { get; protected set; }
    public TransactionTypes TransactionType { get; protected set; }

    /// <summary>
    /// Reserved - Initial Issuer
    /// </summary>
    public string IssuingAuthorityAddress { get; protected set; }
    /// <summary>
    /// Reserved - Controllers with privilege to change access writes
    /// </summary>
    public List<string> PrivilegeControllersAddresses { get; protected set; }
    /// <summary>
    /// Reserved - Controllers
    /// </summary>
    public List<string> ControllersAddresses { get; protected set; }
    public AddressValue IssuerAddress { get; protected set; }
    public AddressValue HolderAddress { get; protected set; }

    /// <summary>
    /// This property is only filled for consent transactions. The value will be null for issue transactions.
    /// </summary>
    public AddressValue VerifierAddress { get; protected set; }
    public string Subject { get; protected set; }
    public string IdentifierKey { get; protected set; }
    public string ValueMimeType { get; protected set; }
    public byte[] Value { get; protected set; }
    public byte[] DoubleEncryptedData { get; set; }
        
    private void ExtractData(byte[] rawData)
    {
        var span = new ReadOnlySpan<byte>(rawData);
        var index = 0;
        TransactionHash = new HashValue(span.Slice(index, HashValue.HashByteLength).ToArray());
        index += HashValue.HashByteLength;
        TransactionType = (TransactionTypes) span.Slice(index, 1)[0];
        index++;
        IssuerAddress = new AddressValue(span.Slice(index, IdCard.PublicKeyByteLength).ToArray());
        index += IdCard.PublicKeyByteLength;
        HolderAddress = new AddressValue(span.Slice(index, IdCard.PublicKeyByteLength).ToArray());
        index += IdCard.PublicKeyByteLength;
        var verifierAddressBytes = span.Slice(index, IdCard.PublicKeyByteLength).ToArray();
        VerifierAddress = verifierAddressBytes.Any(b => b != 0) ? new AddressValue(verifierAddressBytes) : AddressValue.Empty;

        index += IdCard.PublicKeyByteLength;
        Subject = Encoding.ASCII.GetString(span.Slice(index, FrameworkEnvironment.SubjectByteLength)).Trim();
        index += FrameworkEnvironment.SubjectByteLength;
        IdentifierKey = Encoding.ASCII.GetString(span.Slice(index, FrameworkEnvironment.IdentifierByteLength)).Trim();
        index += FrameworkEnvironment.IdentifierByteLength;
        ValueMimeType = ByteArrayToString(span.Slice(index, MimeTypeLength).ToArray());
        index += MimeTypeLength;
            
        Value = span.Slice(index).ToArray();
    }
    
    
    protected static byte[] StringToByteArray(string input, int fixLength) => input.PadRight(fixLength).Select(c => (byte)c).ToArray();
    protected static string ByteArrayToString(byte[] input) => new string(input.Select(b => (char)b).ToArray()).Trim();
}