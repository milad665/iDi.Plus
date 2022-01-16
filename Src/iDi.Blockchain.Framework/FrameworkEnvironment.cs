using System.Security.Cryptography;
using SHA3.Net;

namespace iDi.Blockchain.Framework
{
    public static class FrameworkEnvironment
    {
        public static HashAlgorithm HashAlgorithm => Sha3.Sha3256();
        public const int DefaultServerPort = 28694;
    }
}