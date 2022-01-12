using System.Text;
using iDi.Blockchain.Framework.Cryptography;
using Xunit;

namespace iDi.Blockchain.Framework.Tests.Cryptography;

public class CryptoServiceProviderTests
{
    [Fact]
    public void DataRsaEncryptedByPrivateKeyCanBeDecryptedByPublicKey()
    {
        var rsaKeys = IdCard.Generate();

        var target = new CryptoServiceProvider();
        var messageToEncrypt = "This is a Test MESSAGE!!";
        var encrypted = target.EncryptByPrivateKey(Encoding.UTF8.GetBytes(messageToEncrypt), rsaKeys.PrivateKey);
        var decrypted = target.DecryptByPublicKey(encrypted, rsaKeys.PublicKey);
        var decryptedMessage = Encoding.UTF8.GetString(decrypted);
        Assert.Equal(messageToEncrypt, decryptedMessage);
    }

    [Fact]
    public void DataRsaEncryptedByPublicKeyCanBeDecryptedByPrivateKey()
    {
        var rsaKeys = IdCard.Generate();

        var target = new CryptoServiceProvider();
        var messageToEncrypt = "This is a Test MESSAGE!!";
        var encrypted = target.EncryptByPublicKey(Encoding.UTF8.GetBytes(messageToEncrypt), rsaKeys.PublicKey);
        var decrypted = target.DecryptByPrivateKey(encrypted, rsaKeys.PrivateKey);
        var decryptedMessage = Encoding.UTF8.GetString(decrypted);
        Assert.Equal(messageToEncrypt, decryptedMessage);
    }

    [Fact]
    public void EcdSaSignedDataCanBeVerifiedWithTheMatchingKey()
    {
        var dsKeys = DigitalSignatureKeys.Generate();

        var target = new CryptoServiceProvider();
        var messageToEncrypt = "This is a Test MESSAGE!!";
        var originalData = Encoding.UTF8.GetBytes(messageToEncrypt);

        var signature = target.Sign(dsKeys.PrivateKey, originalData);
        var verified = target.Verify(dsKeys.PublicKey,originalData,signature);
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