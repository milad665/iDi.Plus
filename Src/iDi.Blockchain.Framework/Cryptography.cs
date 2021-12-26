using System.Security.Cryptography;

namespace iDi.Blockchain.Framework
{
    public static class Cryptography
    {
        public const int WalletAddressByteLengthExcludingPrefix = 21;
        public const string WalletAddressPrefix = "IDI";

        public static HashAlgorithm HashAlgorithm => SHA256.Create();
    }
}