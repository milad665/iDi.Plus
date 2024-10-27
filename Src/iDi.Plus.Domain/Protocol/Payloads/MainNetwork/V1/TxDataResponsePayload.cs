using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Plus.Domain.Blockchain.IdTransactions;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

/// <summary>
/// Contains data of a specific verified transaction
/// </summary>
public class TxDataResponsePayload : TxDataRequestPayload
{
    public TxDataResponsePayload(byte[] rawData):base(rawData)
    {
        MessageType = MessageTypes.TxDataResponse;
        ExtractData(rawData);
    }

    private TxDataResponsePayload(HashValue transactionHash, TransactionTypes transactionType, AddressValue issuerAddress, AddressValue holderAddress, AddressValue verifierAddress, string subject, string identifierKey, DateTime timestamp, HashValue previousTransactionHash, string valueMimeType, byte[] value, byte[] rawData) : base(rawData)
    {
        MessageType = MessageTypes.TxDataResponse;

        TransactionHash = transactionHash;
        TransactionType = transactionType;
        IssuerAddress = issuerAddress;
        HolderAddress = holderAddress;
        VerifierAddress = verifierAddress;
        Subject = subject;
        IdentifierKey = identifierKey;
        Timestamp = timestamp;
        PreviousTransactionHash = previousTransactionHash;
        Value = value;
        ValueMimeType = valueMimeType;
    }

    public static TxDataResponsePayload Create(HashValue transactionHash, TransactionTypes transactionType, AddressValue issuerAddress, AddressValue holderAddress, AddressValue verifierAddress, string subject, string identifierKey, DateTime timestamp, HashValue previousTransactionHash, string mimeType, byte[] value)
    {
        if (subject.Length > FrameworkEnvironment.SubjectByteLength)
            throw new InvalidInputException($"Subject length can not be over {FrameworkEnvironment.SubjectByteLength} characters");

        if (identifierKey.Length > FrameworkEnvironment.IdentifierByteLength)
            throw new InvalidInputException($"IdentifierKey length can not be over {FrameworkEnvironment.IdentifierByteLength} characters");

        if (previousTransactionHash == null)
            previousTransactionHash = HashValue.Empty;

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
        lstBytes.AddRange(BitConverter.GetBytes(timestamp.Ticks));
        lstBytes.AddRange(previousTransactionHash.Bytes);
        lstBytes.AddRange(StringToByteArray(mimeType, MimeTypeLength));
        lstBytes.AddRange(value);

        return new TxDataResponsePayload(transactionHash, transactionType, issuerAddress, holderAddress, verifierAddress,
            subject, identifierKey, timestamp, previousTransactionHash, mimeType, value, lstBytes.ToArray());
    }

    public static TxDataResponsePayload FromIdTransaction(IdTransaction tx) => Create(tx.TransactionHash,
        tx.TransactionType, tx.IssuerAddress, tx.HolderAddress,
        tx is ConsentIdTransaction ctx ? ctx.VerifierAddress : null, tx.Subject, tx.IdentifierKey, tx.Timestamp,
        tx.PreviousTransactionHash, tx.ValueMimeType, tx.DoubleEncryptedData);

    public DateTime Timestamp { get; private set; }
    public HashValue PreviousTransactionHash { get; private set; }
        
    private void ExtractData(byte[] rawData)
    {
        var span = new ReadOnlySpan<byte>(rawData);
        var index = 0;
        TransactionHash = new HashValue(span.Slice(index, HashValue.HashByteLength).ToArray());
        index += HashValue.HashByteLength;
        TransactionType = (TransactionTypes) span.Slice(index, 1)[0];
        index++;
        IssuerAddress = new AddressValue(span.Slice(index, IdCard.PublicKeyLength).ToArray());
        index += IdCard.PublicKeyLength;
        HolderAddress = new AddressValue(span.Slice(index, IdCard.PublicKeyLength).ToArray());
        index += IdCard.PublicKeyLength;
        var verifierAddressBytes = span.Slice(index, IdCard.PublicKeyLength).ToArray();
        VerifierAddress = verifierAddressBytes.Any(b => b != 0) ? new AddressValue(verifierAddressBytes) : AddressValue.Empty;

        index += IdCard.PublicKeyLength;
        Subject = Encoding.ASCII.GetString(span.Slice(index, FrameworkEnvironment.SubjectByteLength)).Trim();
        index += FrameworkEnvironment.SubjectByteLength;
        IdentifierKey = Encoding.ASCII.GetString(span.Slice(index, FrameworkEnvironment.IdentifierByteLength)).Trim();
        index += FrameworkEnvironment.IdentifierByteLength;
        Timestamp = DateTime.FromBinary(BitConverter.ToInt64(span.Slice(index, 8)));
        index += 8;
        PreviousTransactionHash = new HashValue(span.Slice(index, HashValue.HashByteLength).ToArray());
        index += HashValue.HashByteLength;
        ValueMimeType = ByteArrayToString(span.Slice(index, MimeTypeLength).ToArray());
        index += MimeTypeLength;
            
        Value = span.Slice(index).ToArray();
    }
}