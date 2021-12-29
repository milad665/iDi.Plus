﻿namespace iDi.Blockchain.Framework.Protocol.iDiDirect
{
    public enum MessageTypes
    {
        Empty = 0,
        /// <summary>
        /// An issuer or holder asks a verifying node in the network to create a new transaction
        /// </summary>
        CreateTx,
        /// <summary>
        /// Send list of new verified transaction hashes to other nodes
        /// </summary>
        NewTxs,
        /// <summary>
        /// Request a specific verified transaction data from a node
        /// </summary>
        GetTx,
        /// <summary>
        /// Send data of a specific verified transaction. (Normally in response to a "GetTx" command)
        /// </summary>
        TxData,
        /// <summary>
        /// Send list of new blocks to other nodes
        /// </summary>
        NewBlocks,
        /// <summary>
        /// Request a specific block from a node
        /// </summary>
        GetBlock,
        /// <summary>
        /// Send data of a specific block. (Normally in response to a "GetBlock" command)
        /// </summary>
        BlockData,
    }
}