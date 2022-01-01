namespace iDi.Blockchain.Framework.Cryptography;

public abstract class KeyPair
{
    protected KeyPair()
    {}

    protected KeyPair(byte[] privateKey, byte[] publicKey)
    {
        PrivateKey = privateKey;
        PublicKey = publicKey;
    }

    public byte[] PrivateKey { get; private set; }
    public byte[] PublicKey { get; private set; }

    /// <summary>
    /// Converts the key pair to a two level password protected PKCS#12 binary data
    /// The two-level passwords enables clients to achieve higher security levels by storing them separately
    /// </summary>
    /// <param name="l1Password">Level-1 password</param>
    /// <param name="l2Password">Level-2 password</param>
    /// <returns></returns>
    public byte[] ToPkcs12(string l1Password, string l2Password)
    {
        var pkcs12Provider = new Pkcs12Provider();
        return pkcs12Provider.ToPkcs12Content(PrivateKey, PublicKey, l1Password, l2Password);
    }

    public string PublicKeyToHexString()
    {
        var provider = new CryptoServiceProvider();
        return provider.KeyToHexString(PublicKey);
    }
}