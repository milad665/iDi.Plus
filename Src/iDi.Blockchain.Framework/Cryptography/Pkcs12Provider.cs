using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;

namespace iDi.Blockchain.Framework.Cryptography;

internal class Pkcs12Provider
{
    public (byte[] PrivateKey, byte[] PublicKey) FromPkcs12Content(byte[] pkcs12Data, string l1Password, string l2Password)
    {
        var info = Pkcs12Info.Decode(pkcs12Data, out _);
        if (!info.VerifyMac(l1Password))
            throw new UnauthorizedAccessException("Invalid l1Password");

        var safe = info.AuthenticatedSafe.First();
        safe.Decrypt(l2Password);
        var privateKey = safe.GetBags().First().EncodedBagValue.ToArray();
        var publicKey = safe.GetBags().Skip(1).First().EncodedBagValue.ToArray();
        return new (privateKey, publicKey);
    }

    public byte[] ToPkcs12Content(byte[] privateKey, byte[] publicKey, string l1Password, string l2Password)
    {
        var content = new Pkcs12SafeContents();
        content.AddSafeBag(new Pkcs12KeyBag(privateKey));
        content.AddSafeBag(new Pkcs12KeyBag(publicKey));
        var builder = new Pkcs12Builder();
        builder.AddSafeContentsEncrypted(content, l2Password, new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, 3));
        builder.SealWithMac(l1Password, HashAlgorithmName.SHA256, 3);
        return builder.Encode();
    }
}