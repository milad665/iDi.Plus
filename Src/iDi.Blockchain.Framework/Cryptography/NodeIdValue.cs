using System;
using System.Linq;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Protocol.Extensions;

namespace iDi.Blockchain.Framework.Cryptography;

public class NodeIdValue
{
    public const int NodeIdByteLength = DigitalSignatureKeys.PublicKeyByteLength;

    public NodeIdValue(string hexHexString)
    {
        if (string.IsNullOrWhiteSpace(hexHexString))
            throw new InvalidInputException("Hash cannot be empty.");

        var bytes = hexHexString.HexStringToByteArray();
        if (bytes.Length != NodeIdByteLength)
            throw new InvalidInputException("Invalid hash length.");

        HexString = hexHexString;
        Bytes = bytes;
    }

    public NodeIdValue(byte[] bytes)
    {
        if (bytes.Length != NodeIdByteLength)
            throw new InvalidInputException("Invalid hash length.");

        HexString = bytes.All(b => b == 0) ? null : bytes.ToHexString();
        Bytes = bytes;
    }

    protected NodeIdValue(string hexString, byte[] bytes)
    {
        HexString = hexString;
        Bytes = bytes;
    }

    public string HexString { get; }
    public byte[] Bytes { get; }

    public override bool Equals(object obj)
    {
        if (obj is not NodeIdValue hashObj)
            return false;

        if (string.IsNullOrWhiteSpace(HexString))
            return string.IsNullOrWhiteSpace(hashObj.HexString);

        return HexString.Equals(hashObj.HexString, StringComparison.OrdinalIgnoreCase);
    }

    protected bool Equals(NodeIdValue other)
    {
        if (other is null)
            return false;

        if (string.IsNullOrWhiteSpace(HexString))
            return string.IsNullOrWhiteSpace(other.HexString);

        return HexString.Equals(other.HexString, StringComparison.OrdinalIgnoreCase);
    }

    public static bool operator ==(NodeIdValue value1, NodeIdValue value2) => value1?.Equals(value2) ?? (value2 is null);

    public static bool operator !=(NodeIdValue value1, NodeIdValue value2) => !(value1 == value2);

    public override int GetHashCode()
    {
        return HashCode.Combine(HexString.ToLower(), Bytes);
    }
    public override string ToString()
    {
        return HexString?.ToLower();
    }
}