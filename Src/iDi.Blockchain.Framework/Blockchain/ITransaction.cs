﻿using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Blockchain.Framework.Blockchain;

/// <summary>
/// Abstracts the logic behind a transaction
/// </summary>
public interface ITransaction
{
    /// <summary>
    /// Hash of the transaction
    /// </summary>
    HashValue TransactionHash { get;}
    
    /// <summary>
    /// Type of the transaction
    /// </summary>
    TransactionTypes TransactionType { get;  }
}