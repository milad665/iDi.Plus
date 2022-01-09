﻿using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Exceptions;

namespace iDi.Blockchain.Framework.Providers;

public class LocalNodeContextProvider
{
    private KeyPair _localKeyPair;

    public KeyPair LocalKeys
    {
        get => _localKeyPair;
        set
        {
            if (_localKeyPair != null)
                throw new InvalidDataException("Node keys have been set before.");

            _localKeyPair = value;
        }
    }

    public bool IsBlockchainUpToDate { get; private set; }

    public void SetBlockchainUpToDate()
    {
        IsBlockchainUpToDate = true;
    }


}