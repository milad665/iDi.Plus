using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Protocol.Processors;

public class GetWitnessNodesMessageProcessor : MessageProcessorBase
{
    public GetWitnessNodesMessageProcessor(IBlockchainNodeClient blockchainNodeClient, 
        IIdBlockchainRepository idBlockchainRepository, 
        IHotPoolRepository<IdTransaction> hotPoolRepository, 
        ILocalNodeContextProvider localNodeContextProvider, 
        IBlockchainNodesRepository blockchainNodesRepository) 
        : base(blockchainNodeClient, idBlockchainRepository, hotPoolRepository, localNodeContextProvider, blockchainNodesRepository)
    {
    }

    public override MessageTypes MessageType => MessageTypes.GetWitnessNodes;

    protected override Message ProcessPayload(Message message)
    {
        var cryptoServiceProvider = new CryptoServiceProvider();

        var nodes = BlockchainNodesRepository.GetWitnessNodes();
        var payload = WitnessNodesList.Create(nodes);
        var header = Header.Create(message.Header.Network, message.Header.Version,
            LocalNodeContextProvider.LocalNodeId(), MessageTypes.WitnessNodesList, payload.RawData.Length,
            cryptoServiceProvider.Sign(LocalNodeContextProvider.LocalKeys.PrivateKey, payload.RawData));

        return Message.Create(header,payload);
    }
}