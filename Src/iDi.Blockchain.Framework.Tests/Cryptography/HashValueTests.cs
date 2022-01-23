using System;
using System.Linq;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using Xunit;

namespace iDi.Blockchain.Framework.Tests.Cryptography;

public class HashValueTests
{
    private readonly string _sampleHashHexString = "ed2333efd969e0961cbbc34e1fa794cba0e8e2d0d134a64bc88adb6421e80f6a";
    private readonly byte[] _sampleHashBytes = {
        237, 35, 51, 239, 217, 105, 224, 150, 28, 187, 195, 78, 31, 167, 148, 203, 160, 232, 226, 208, 209, 52, 166, 75,
        200, 138, 219, 100, 33, 232, 15, 106
    };
    private readonly byte[] _sampleEmptyHashBytes = {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0
    };

    [Fact]
    public void SuccessfulCreationFromHexString()
    {
        var target = new HashValue(_sampleHashHexString);
        Assert.Equal(_sampleHashBytes.Length, target.Bytes.Length);
        for (var i=0; i< target.Bytes.Length;i++)
            Assert.Equal(_sampleHashBytes[i], target.Bytes[i]);
    }

    [Fact]
    public void SuccessfulCreationFromByteArray()
    {
        var target = new HashValue(_sampleHashHexString);
        Assert.Equal(_sampleHashHexString, target.HexString, true);
    }

    [Fact]
    public void ThrowsError_InvalidHashByteLength()
    {
        var bytes = _sampleHashBytes.Take(26).ToArray();
        Assert.Throws<InvalidInputException>(() => new HashValue(bytes));
    }

    [Fact]
    public void ThrowsError_InvalidHashHexStringLength()
    {
        var hexString = _sampleHashHexString.Substring(26);
        Assert.Throws<InvalidInputException>(() => new HashValue(hexString));
    }

    [Fact]
    public void ThrowsError_InvalidHashHexString()
    {
        var hexString = "INVALIDHASH";
        Assert.Throws<FormatException>(() => new HashValue(hexString));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ThrowsError_NullHashHexString(string hexString)
    {
        Assert.Throws<InvalidInputException>(() => new HashValue(hexString));
    }

    [Fact]
    public void EmptyHashIsCreatedFromNullBytes()
    {
        var target = new HashValue(_sampleEmptyHashBytes);
        Assert.True(target.IsEmpty());
        Assert.Null(target.HexString);
        foreach (var b in target.Bytes)
            Assert.Equal(0, b);
    }

    [Fact]
    public void EmptyHashIsCreatedSuccessfully()
    {
        var target = HashValue.Empty;
        Assert.True(target.IsEmpty());
        Assert.Equal(HashValue.HashByteLength, target.Bytes.Length);
        foreach (var b in target.Bytes)
            Assert.Equal(0, b);
        
        Assert.Null(target.HexString);
    }
}