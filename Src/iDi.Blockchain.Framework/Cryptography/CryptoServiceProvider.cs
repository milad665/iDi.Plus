using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;

namespace iDi.Blockchain.Framework.Cryptography
{
    /// <summary>
    /// Provides cryptographic services based on RSA algorithm
    /// </summary>
    public class CryptoServiceProvider
    {
        /// <summary>
        /// Signs the data. This mean the data is encrypted with private key
        /// </summary>
        /// <param name="bytesToSign"></param>
        /// <param name="privateKey">RSA Private Key</param>
        /// <returns></returns>
        public byte[] EncryptByPrivateKey(byte[] bytesToSign, byte[] privateKey)
        {
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());
            var keyParameter = PrivateKeyFactory.CreateKey(privateKey);
            encryptEngine.Init(true, keyParameter);

            return encryptEngine.ProcessBlock(bytesToSign, 0, bytesToSign.Length);
        }

        /// <summary>
        /// Verifies signed data. This means the data previously encrypted with private key will be decrypted with the public key
        /// </summary>
        /// <param name="bytesToVerify"></param>
        /// <param name="publicKey">RSA Public Key</param>
        /// <returns></returns>
        public byte[] DecryptByPublicKey(byte[] bytesToVerify, byte[] publicKey)
        {
            var decryptEngine = new Pkcs1Encoding(new RsaEngine());
            var keyParameter = PublicKeyFactory.CreateKey(publicKey);
            decryptEngine.Init(false, keyParameter);
            
            return decryptEngine.ProcessBlock(bytesToVerify, 0, bytesToVerify.Length);
        }

        /// <summary>
        /// Encrypts the data with public key
        /// </summary>
        /// <param name="bytesToEncrypt"></param>
        /// <param name="publicKey">RSA Public Key</param>
        /// <returns></returns>
        public byte[] EncryptByPublicKey(byte[] bytesToEncrypt, byte[] publicKey)
        {
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());
            var keyParameter = PublicKeyFactory.CreateKey(publicKey);
            encryptEngine.Init(true, keyParameter);

            return encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length);
        }

        /// <summary>
        /// Decrypts the encrypted data with private key
        /// </summary>
        /// <param name="bytesToDecrypt"></param>
        /// <param name="privateKey">RSA Private Key</param>
        /// <returns></returns>
        public byte[] DecryptByPrivateKey(byte[] bytesToDecrypt, byte[] privateKey)
        {
            var decryptEngine = new Pkcs1Encoding(new RsaEngine());
            var keyParameter = PrivateKeyFactory.CreateKey(privateKey);
            decryptEngine.Init(false, keyParameter);

            return decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length);
        }

        /// <summary>
        /// Signs the data, so it's origin can later be verified.
        /// </summary>
        /// <param name="privateKey">ECDSA private key of the origin</param>
        /// <param name="dataToSign"></param>
        /// <returns></returns>
        public byte[] Sign(byte[] privateKey, byte[] dataToSign)
        {
            var ecdsa = ECDsa.Create(CryptographyConstants.EcDsaCurve);
            ecdsa.ImportECPrivateKey(privateKey, out _);

            return ecdsa.SignData(dataToSign, HashAlgorithmName.SHA256);
        }

        /// <summary>
        /// Verifies the data against its signature to validate its origin
        /// </summary>
        /// <param name="publicKey">ECDSA public key of the origin</param>
        /// <param name="data">Data of which the origin is to be verified</param>
        /// <param name="signature">Digital signature of the data</param>
        /// <returns></returns>
        public bool Verify(byte[] publicKey, byte[] data, byte[] signature)
        {
            var ecdsa = ECDsa.Create(CryptographyConstants.EcDsaCurve);

            ecdsa.ImportSubjectPublicKeyInfo(publicKey, out _);
            return ecdsa.VerifyData(data, signature, HashAlgorithmName.SHA256);
        }
    }
}
