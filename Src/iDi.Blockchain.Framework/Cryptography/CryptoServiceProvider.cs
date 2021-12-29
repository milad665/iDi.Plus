using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;
using System;

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
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public byte[] Sign(byte[] bytesToSign, byte[] privateKey)
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
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public byte[] Verify(byte[] bytesToVerify, byte[] publicKey)
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
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] bytesToEncrypt, byte[] publicKey)
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
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] bytesToDecrypt, byte[] privateKey)
        {
            var decryptEngine = new Pkcs1Encoding(new RsaEngine());
            var keyParameter = PrivateKeyFactory.CreateKey(privateKey);
            decryptEngine.Init(false, keyParameter);

            return decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length);
        }

        /// <summary>
        /// Converts byte array to Hex-String
        /// </summary>
        /// <param name="btyes"></param>
        /// <returns></returns>
        public string KeyToHexString(byte[] btyes)
        {
            return $"0x{Convert.ToHexString(btyes)}";
        }

        /// <summary>
        /// Converts Hex-String to byte array
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public byte[] HexStringToKey(string hexString)
        {
            hexString = hexString.Replace("0x", "");
            return Convert.FromHexString(hexString);
        }
    }
}
