using System;

namespace iDi.Blockchain.Framework.Protocol.iDiDirect.Extensions
{
    public static class HexStringExtensions
    {
        public static byte[] HexStringToByteArray(this string hexString)
        {
            var numberChars = hexString.Length;
            var bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            return bytes;
        }

        public static string ToHexString(this byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "").ToUpper();
        }

        public static string ToHexString(this ReadOnlySpan<byte> data)
        {
            return ToHexString(data.ToArray());
        }
    }
}