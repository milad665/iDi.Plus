using iDi.Blockchain.Framework.Server;

namespace iDi.Plus.Application
{
    public class Process
    {
        private readonly Settings _settings;
        private readonly IBlockchainNodeServer _blockchainNodeServer;
        public Process(Settings settings, IBlockchainNodeServer blockchainNodeServer)
        {
            _settings = settings;
            _blockchainNodeServer = blockchainNodeServer;
        }

        public void Run()
        {
            // Load DNSs
            // Update node addresses (Max 1000 random)
            // Download/Update blockchain

            _blockchainNodeServer.Listen(_settings.Port);
        }
    }
}