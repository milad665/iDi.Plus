using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using SHA3.Net;
using System;
using System.Linq;

namespace iDi.Blockchain.Framework.Cryptography
{
    public class IdCard
    {
        protected IdCard()
        {}

        protected IdCard(byte[] privateKey, byte[] publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;
            Address = GetAddressFromPublicKey(publicKey);
        }

        public static IdCard Generate()
        {
            var gen = new RsaKeyPairGenerator();
            var secureRandom = new SecureRandom();
            var keyGenParam = new KeyGenerationParameters(secureRandom, 3072);
            gen.Init(keyGenParam);

            var keys = gen.GenerateKeyPair();
            var pubF = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keys.Public);
            byte[] publicKey = pubF.GetDerEncoded();

            var privF = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keys.Private);
            byte[] privateKey = privF.GetDerEncoded();

            return new IdCard(privateKey, publicKey);
        }

        public byte[] PrivateKey { get; private set; }
        public byte[] PublicKey { get; private set; }
        public string Address { get; private set; }

        private string GetAddressFromPublicKey(byte[] publicKey)
        {
            var hash = CryptographyConstants.HashAlgorithm.ComputeHash(publicKey);
            return $"IDI{Convert.ToHexString(hash.Take(21).ToArray())}";
        }
    }
}
