using System;
using System.Linq;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Protocol.Extensions;

namespace iDi.Blockchain.Framework.Cryptography;

public class AddressValue
{
    public const int AddressByteLength = IdCard.PublicKeyByteLength;

    protected AddressValue()
    { }

    public AddressValue(string hexHexString)
    {
        if (string.IsNullOrWhiteSpace(hexHexString))
            throw new InvalidInputException("Address cannot be empty.");

        var bytes = hexHexString.HexStringToByteArray();
        if (bytes.Length != AddressByteLength)
            throw new InvalidInputException("Invalid address length.");

        HexString = hexHexString;
        Bytes = bytes;
    }

    public AddressValue(byte[] bytes)
    {
        if (bytes.Length != AddressByteLength)
            throw new InvalidInputException("Invalid address length.");

        HexString = bytes.All(b => b == 0) ? null : bytes.ToHexString();
        Bytes = bytes;
    }

    protected AddressValue(string hexString, byte[] bytes)
    {
        HexString = hexString;
        Bytes = bytes;
    }

    public static AddressValue Empty => new() { Bytes = new byte[AddressByteLength] };

    public string HexString { get; }
    public byte[] Bytes { get; private init; }

    public bool IsEmpty() => string.IsNullOrWhiteSpace(HexString);

    public static bool IsValidAddress(string walletAddress)
    {
        if (string.IsNullOrWhiteSpace(walletAddress))
            return false;

        try
        {
            var bytes = walletAddress.HexStringToByteArray();
            return bytes.Length == AddressByteLength;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj is not AddressValue addressObj)
            return false;

        if (IsEmpty() && addressObj.IsEmpty())
            return true;

        if (string.IsNullOrWhiteSpace(HexString))
            return string.IsNullOrWhiteSpace(addressObj.HexString);

        return HexString.Equals(addressObj.HexString, StringComparison.OrdinalIgnoreCase);
    }

    protected bool Equals(AddressValue other)
    {
        if (other is null)
            return false;

        if (string.IsNullOrWhiteSpace(HexString))
            return string.IsNullOrWhiteSpace(other.HexString);

        return HexString.Equals(other.HexString, StringComparison.OrdinalIgnoreCase);
    }

    public static bool operator ==(AddressValue value1, AddressValue value2) => value1?.Equals(value2) ?? (value2 is null);

    public static bool operator !=(AddressValue value1, AddressValue value2) => !(value1 == value2);

    public override int GetHashCode()
    {
        return HashCode.Combine(HexString.ToLower(), Bytes);
    }
    public override string ToString()
    {
        return HexString?.ToLower();
    }
}