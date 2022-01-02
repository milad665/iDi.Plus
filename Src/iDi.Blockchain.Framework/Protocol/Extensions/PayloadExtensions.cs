using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Blockchain.Framework.Protocol.Extensions;

public static class PayloadExtensions
{
    public static byte[] Sign(this IPayload payload, byte[] privateKey)
    {
        var cryptoServiceProvider = new CryptoServiceProvider();
        return cryptoServiceProvider.Sign(privateKey, payload.RawData);
    }
}