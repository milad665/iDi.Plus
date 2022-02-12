using System;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Exceptions;
using Xunit;

namespace iDi.Blockchain.Framework.Tests.Cryptography;

public class AddressValueTests
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("162fe433afd216")] //Too small
    public void ThrowsError_InvalidAddress(string invalidAddress)
    {
        Assert.Throws<InvalidInputException>(() => new AddressValue(invalidAddress));
    }

    [Fact]
    public void ThrowsError_InvalidHexStringAddress()
    {
        Assert.Throws<FormatException>(() => new AddressValue("Address"));
    }
}