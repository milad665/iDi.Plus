namespace iDi.Blockchain.Core.Messages
{
    public enum MessageTypes
    {
        Empty = 0,
        /// <summary>
        /// Send list of new transaction hashes to other nodes
        /// </summary>
        NewTxs,
        /// <summary>
        /// Request a specific transaction data from a node
        /// </summary>
        GetTx,
        /// <summary>
        /// Send data of a specific transaction. (Normally in response to a "GetTx" command)
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