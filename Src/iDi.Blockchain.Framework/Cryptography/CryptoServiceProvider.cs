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
        /// Encrypts the data with private key using RSA algorithm. (Signs the data)
        /// </summary>
        /// <param name="bytesToEncrypt"></param>
        /// <param name="privateKey">RSA Private Key</param>
        /// <returns></returns>
        public byte[] EncryptByPrivateKey(byte[] bytesToEncrypt, byte[] privateKey)
        {
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());
            var keyParameter = PrivateKeyFactory.CreateKey(privateKey);
            encryptEngine.Init(true, keyParameter);

            return encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length);
        }

        /// <summary>
        /// Data previously encrypted with private key will be decrypted with the public key using RSA algorithm. (Verifies signed data)
        /// </summary>
        /// <param name="bytesToDecrypt"></param>
        /// <param name="publicKey">RSA Public Key</param>
        /// <returns></returns>
        public byte[] DecryptByPublicKey(byte[] bytesToDecrypt, byte[] publicKey)
        {
            var decryptEngine = new Pkcs1Encoding(new RsaEngine());
            var keyParameter = PublicKeyFactory.CreateKey(publicKey);
            decryptEngine.Init(false, keyParameter);
            
            return decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length);
        }

        /// <summary>
        /// Encrypts the data with public key using RSA algorithm.
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
        /// Decrypts previously encrypted data with private key using RSA algorithm.
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
        /// Signs the data using ECDSa, so it's origin can later be verified.
        /// </summary>
        /// <param name="privateKey">ECDSa private key of the origin</param>
        /// <param name="dataToSign"></param>
        /// <returns></returns>
        public byte[] Sign(byte[] privateKey, byte[] dataToSign)
        {
            if (dataToSign == null)
                return null;

            var ecdsa = ECDsa.Create(FrameworkEnvironment.EcDsaCurve);
            ecdsa.ImportECPrivateKey(privateKey, out _);

            return ecdsa.SignData(dataToSign, HashAlgorithmName.SHA256);
        }

        /// <summary>
        /// Verifies the data using ECDSa against its signature to validate its origin
        /// </summary>
        /// <param name="publicKey">ECDSa public key of the origin</param>
        /// <param name="data">Data of which the origin is to be verified</param>
        /// <param name="signature">Digital signature of the data</param>
        /// <returns></returns>
        public bool Verify(byte[] publicKey, byte[] data, byte[] signature)
        {
            var ecDsa = ECDsa.Create(FrameworkEnvironment.EcDsaCurve);

            ecDsa.ImportSubjectPublicKeyInfo(publicKey, out _);
            return ecDsa.VerifyData(data, signature, HashAlgorithmName.SHA256);
        }
    }
}
