namespace iDi.Blockchain.Framework.Protocol
{
    public enum MessageTypes : byte
    {
        Empty = 0,

        /// <summary>
        /// An issuer or holder asks a witness node in the network to create a new transaction
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
        /// Request the list of new blocks added after a specific timestamp to the blockchain
        /// </summary>
        GetNewBlocks,
        
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

        /// <summary>
        /// Vote to choose the node to create the next block
        /// </summary>
        Vote,

        /// <summary>
        /// Request the list of all witness and dns nodes
        /// </summary>
        GetWitnessNodes,

        /// <summary>
        /// List of all witness and dns nodes
        /// </summary>
        WitnessNodesList,
    }
}