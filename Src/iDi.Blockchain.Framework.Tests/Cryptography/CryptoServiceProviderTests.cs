using System;
using System.Text;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Extensions;
using Xunit;

namespace iDi.Blockchain.Framework.Tests.Cryptography;

public class CryptoServiceProviderTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(300)]
    [InlineData(372)]
    [InlineData(373)]//buffer edge
    [InlineData(375)]
    [InlineData(382)]
    [InlineData(383)]
    [InlineData(384)]//buffer edge
    [InlineData(385)]
    [InlineData(500)]
    [InlineData(5000)]
    [InlineData(50000)]
    public void DataRsaEncryptedByPrivateKeyCanBeDecryptedByPublicKey(int dataLength)
    {
        var rsaKeys = IdCard.Generate();
        var data = new byte[dataLength];
        var rnd = new Random(DateTime.Now.Millisecond);
        rnd.NextBytes(data);

        var target = new CryptoServiceProvider();
        var encrypted = target.EncryptByPrivateKey(data, rsaKeys.PrivateKey);
        var decrypted = target.DecryptByPublicKey(encrypted, rsaKeys.PublicKey);
        Assert.Equal(data.ToHexString(), decrypted.ToHexString(), true);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(300)]
    [InlineData(372)]
    [InlineData(373)]//buffer edge
    [InlineData(375)]
    [InlineData(382)]
    [InlineData(383)]
    [InlineData(384)]//buffer edge
    [InlineData(385)]
    [InlineData(500)]
    [InlineData(5000)]
    [InlineData(50000)]
    public void DataRsaEncryptedByPublicKeyCanBeDecryptedByPrivateKey(int dataLength)
    {
        var rsaKeys = IdCard.Generate();
        var data = new byte[dataLength];
        var rnd = new Random(DateTime.Now.Millisecond);
        rnd.NextBytes(data);

        var target = new CryptoServiceProvider();
        var encrypted = target.EncryptByPublicKey(data, rsaKeys.PublicKey);
        var decrypted = target.DecryptByPrivateKey(encrypted, rsaKeys.PrivateKey);
        Assert.Equal(data.ToHexString(), decrypted.ToHexString(), true);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(300)]
    [InlineData(500)]
    [InlineData(5000)]
    [InlineData(50000)]
    public void EcdSaSignedDataCanBeVerifiedWithTheMatchingKey(int dataLength)
    {
        var dsKeys = DigitalSignatureKeys.Generate();
        var data = new byte[dataLength];
        var rnd = new Random(DateTime.Now.Millisecond);
        rnd.NextBytes(data);

        var target = new CryptoServiceProvider();

        var signature = target.Sign(dsKeys.PrivateKey, data);
        var verified = target.Verify(dsKeys.PublicKey,data,signature);
        Assert.True(verified);
    }

    [Fact]
    public void VerificationFailsIfSignatureDoesNotMatchTheData()
    {
        var dsKeys = DigitalSignatureKeys.Generate();

        var target = new CryptoServiceProvider();
        var messageToEncrypt = "This is a Test MESSAGE!!";
        var originalData = Encoding.UTF8.GetBytes(messageToEncrypt);

        var signature = target.Sign(dsKeys.PrivateKey, originalData);
        var verified = target.Verify(dsKeys.PublicKey, originalData, new byte[signature.Length]);
        Assert.False(verified);
    }
}