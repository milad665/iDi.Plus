using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Blockchain.Framework.Providers;

public interface ILocalNodeContextProvider
{
    KeyPair LocalKeys { get; set; }
    bool IsBlockchainUpToDate { get; }
    void SetBlockchainUpToDate();
}