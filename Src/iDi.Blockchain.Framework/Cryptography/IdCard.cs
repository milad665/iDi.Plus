﻿using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace iDi.Blockchain.Framework.Cryptography
{
    /// <summary>
    /// An object which holds the private and public key pair of an identity. This object is equivalent to a wallet in crypto-currency domain.
    /// </summary>
    public class IdCard : KeyPair
    {
        //public const int IdiCardAddressByteLengthExcludingPrefix = 21;
        //public const string IdCardAddressPrefix = "IDI";

        public const int PrivateKeyLength = 1793;
        public const int PublicKeyLength = 422;
        public const int RsaKeyStrength = 3072;


        protected IdCard()
        {}

        protected IdCard(byte[] privateKey, byte[] publicKey):base(privateKey, publicKey)
        {
            Address = GetAddressFromPublicKey(publicKey);
        }

        /// <summary>
        /// Generate new key pair
        /// </summary>
        /// <returns>IdCard object</returns>
        public static IdCard Generate()
        {
            var gen = new RsaKeyPairGenerator();
            var secureRandom = new SecureRandom();
            var keyGenParam = new KeyGenerationParameters(secureRandom, RsaKeyStrength);
            gen.Init(keyGenParam);

            var keys = gen.GenerateKeyPair();
            var pubF = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keys.Public);
            byte[] publicKey = pubF.GetDerEncoded();

            var privF = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keys.Private);
            byte[] privateKey = privF.GetDerEncoded();

            return new IdCard(privateKey, publicKey);
        }

        /// <summary>
        /// Create an IdCard instance from PKCS#12 binary data
        /// </summary>
        /// <param name="pkcs12Data">PKCS#12 binary data</param>
        /// <param name="l1Password">Level-1 password</param>
        /// <param name="l2Password">Level-2 password</param>
        /// <returns></returns>
        public static IdCard FromPkcs12(byte[] pkcs12Data, string l1Password, string l2Password)
        {
            var pkcs12Provider = new Pkcs12Provider();
            var keys = pkcs12Provider.FromPkcs12Content(pkcs12Data, l1Password, l2Password);
            return new IdCard(keys.PrivateKey, keys.PublicKey);
        }

        /// <summary>
        /// IdCard address generated from the public key
        /// </summary>
        public AddressValue Address { get; private set; }

        private AddressValue GetAddressFromPublicKey(byte[] publicKey)
        {
            //var hash = FrameworkEnvironment.HashAlgorithm.ComputeHash(publicKey);
            //return $"{IdCardAddressPrefix}{Convert.ToHexString(hash.Take(IdiCardAddressByteLengthExcludingPrefix).ToArray())}";

            return new AddressValue(publicKey);
        }
    }
}
