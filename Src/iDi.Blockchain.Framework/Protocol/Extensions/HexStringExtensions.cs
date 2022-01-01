using System;

namespace iDi.Blockchain.Framework.Protocol.Extensions
{
    public static class HexStringExtensions
    {
        public static byte[] HexStringToByteArray(this string hexString)
        {
            return Convert.FromHexString(hexString);
        }

        public static string ToHexString(this byte[] data)
        {
            return Convert.ToHexString(data);
        }
        
        public static string ToHexString(this ReadOnlySpan<byte> data)
        {
            return ToHexString(data.ToArray());
        }
    }
}