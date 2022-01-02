using System.Security.Cryptography;
using SHA3.Net;

namespace iDi.Blockchain.Framework
{
    public static class FrameworkEnvironment
    {
        public const int WalletAddressByteLengthExcludingPrefix = 21;
        public const string WalletAddressPrefix = "IDI";

        public static HashAlgorithm HashAlgorithm => Sha3.Sha3256();
        public static ECCurve EcDsaCurve = ECCurve.NamedCurves.nistP256;
        public static int NodeIdByteLength => 91; //nistP256 public key length
        public const int DefaultServerPort = 28694;
    }
}