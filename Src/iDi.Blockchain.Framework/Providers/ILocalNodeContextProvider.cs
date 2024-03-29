﻿using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Blockchain.Framework.Providers;

public interface ILocalNodeContextProvider
{
    KeyPair LocalKeys { get; set; }
    NodeIdValue LocalNodeId();
    bool IsBlockchainUpToDate { get; }
    public bool IsWitnessNode { get; }
    public bool IsDnsNode { get; }
    void SetBlockchainUpToDate();
    void SetWitnessNode();
    void SetDnsNode();
}