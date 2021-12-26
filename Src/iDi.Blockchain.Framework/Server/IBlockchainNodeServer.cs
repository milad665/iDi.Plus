namespace iDi.Blockchain.Framework.Server
{
    /// <summary>
    /// Abstracts the blockchain node server
    /// </summary>
    public interface IBlockchainNodeServer
    {
        /// <summary>
        /// Starts listening to the specified port
        /// </summary>
        /// <param name="port"></param>
        void Listen(int port);
    }
}
