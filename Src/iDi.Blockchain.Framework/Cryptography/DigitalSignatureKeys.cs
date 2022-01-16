using System.Security.Cryptography;

namespace iDi.Blockchain.Framework.Cryptography;

/// <summary>
/// An object which holds a private and public key pair
/// </summary>
public class DigitalSignatureKeys : KeyPair
{
    public static ECCurve EcDsaCurve = ECCurve.NamedCurves.nistP256;
    public const int NodeIdByteLength = 91; //nistP256 public key length

    protected DigitalSignatureKeys()
    {}

    protected DigitalSignatureKeys(byte[] privateKey, byte[] publicKey) : base(privateKey, publicKey)
    {
    }

    /// <summary>
    /// Generate new key pair
    /// </summary>
    /// <returns>DigitalSignatureKeys object</returns>
    public static DigitalSignatureKeys Generate()
    {
        var ecdsa = ECDsa.Create(EcDsaCurve);

        var privateKey = ecdsa.ExportECPrivateKey();
        var publicKey = ecdsa.ExportSubjectPublicKeyInfo();

        return new DigitalSignatureKeys(privateKey, publicKey);
    }

    /// <summary>
    /// Create an DigitalSignatureKeys instance from PKCS#12 binary data
    /// </summary>
    /// <param name="pkcs12Data">PKCS#12 binary data</param>
    /// <param name="l1Password">Level-1 password</param>
    /// <param name="l2Password">Level-2 password</param>
    /// <returns></returns>
    public static DigitalSignatureKeys FromPkcs12(byte[] pkcs12Data, string l1Password, string l2Password)
    {
        var pkcs12Provider = new Pkcs12Provider();
        var keys = pkcs12Provider.FromPkcs12Content(pkcs12Data, l1Password, l2Password);
        return new DigitalSignatureKeys(keys.PrivateKey, keys.PublicKey);
    }
}