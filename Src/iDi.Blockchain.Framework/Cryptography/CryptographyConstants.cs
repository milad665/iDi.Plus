using SHA3.Net;
using System.Security.Cryptography;

namespace iDi.Blockchain.Framework.Cryptography
{
    public static class CryptographyConstants
    {
        public const int WalletAddressByteLengthExcludingPrefix = 21;
        public const string WalletAddressPrefix = "IDI";

        public static HashAlgorithm HashAlgorithm => Sha3.Sha3256();
        public static ECCurve EcDsaCurve = ECCurve.NamedCurves.nistP256;
        public static int NodeIdByteLength => 91; //nistP256 public key length
    }
}