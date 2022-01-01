using System.Threading;

namespace iDi.Blockchain.Framework.Communication
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
        /// <param name="cancellationToken"></param>
        void Listen(int port, CancellationToken cancellationToken);
    }
}
