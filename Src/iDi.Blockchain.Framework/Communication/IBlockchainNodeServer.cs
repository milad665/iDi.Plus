using System.Collections.ObjectModel;
using System.Threading;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;

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
        /// <param name="nodesList">List of distributed blockchain nodes</param>
        /// <param name="localKeys">Public/Private key pair of the local node</param>
        void Listen(int port, CancellationToken cancellationToken, ReadOnlyDictionary<string, BlockchainNode> nodesList, KeyPair localKeys);
    }
}
