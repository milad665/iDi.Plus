using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using iDi.Blockchain.Framework.Protocol.Extensions;
using SHA3.Net;

namespace iDi.Blockchain.Framework.Cryptography;

public class HashValue
{
    private static readonly HashAlgorithm HashAlgorithm = Sha3.Sha3256();
    public static int HashByteLength = HashAlgorithm.HashSize / 8;

    protected HashValue()
    {}

    public HashValue(string hexHexString)
    {
        var bytes = hexHexString.HexStringToByteArray();
        if (bytes.Length != HashByteLength)
            throw new InvalidDataException("Invalid hash length.");
        HexString = hexHexString;
        Bytes = bytes;
    }

    public HashValue(byte[] bytes)
    {
        if (bytes.Length != HashByteLength)
            throw new InvalidDataException("Invalid hash length.");
        HexString = bytes.All(b => b == 0) ? null : bytes.ToHexString();
        Bytes = bytes;
    }

    public static HashValue ComputeHash(byte[] data)
    {
        var hashedBytes = HashAlgorithm.ComputeHash(data);
        return new HashValue(hashedBytes);
    }

    public static HashValue ComputeHash<T>(T input)
    {
        var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input));
        var hashedBytes = HashAlgorithm.ComputeHash(data);
        return new HashValue(hashedBytes);
    }

    public static HashValue Empty => new(){ Bytes = new byte[HashByteLength] };

    public string HexString { get; }
    public byte[] Bytes { get; private init; }

    public bool IsEmpty() => string.IsNullOrWhiteSpace(HexString);

    public bool EqualsHexString(string hexString)
    {
        return HexString.Equals(hexString, StringComparison.OrdinalIgnoreCase);
    }

    public bool EqualsBytes(byte[] bytes)
    {
        return HexString.Equals(bytes.ToHexString(), StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj)
    {
        if (obj is not HashValue hashObj)
            return false;

        if (IsEmpty() && hashObj.IsEmpty())
            return true;

        return HexString.Equals(hashObj.HexString, StringComparison.OrdinalIgnoreCase);
    }

    protected bool Equals(HashValue other)

    {
        if (other == null)
            return false;

        return HexString.Equals(other.HexString, StringComparison.OrdinalIgnoreCase) && Equals(Bytes, other.Bytes);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(HexString, Bytes);
    }

    public static bool IsValid(string hashHexString)
    {
        try
        {
            var bytes = hashHexString.HexStringToByteArray();
            if (bytes.Length != HashByteLength)
                return false;

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool operator ==(HashValue value1, HashValue value2) => value1?.Equals(value2) ?? value2 is null;

    public static bool operator !=(HashValue value1, HashValue value2) => !(value1 == value2);
}