using System.Linq;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Providers;
using iDi.Plus.Domain.Blockchain;
using iDi.Plus.Domain.Blockchain.IdTransactions;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Protocol.Processors;

public class WitnessNodesListMessageProcessor: MessageProcessorBase
{
    public WitnessNodesListMessageProcessor(IBlockchainNodeClient blockchainNodeClient,
        IIdBlockchainRepository idBlockchainRepository, IHotPoolRepository<IdTransaction> hotPoolRepository,
        ILocalNodeContextProvider localNodeContextProvider, IBlockchainNodesRepository blockchainNodesRepository) :
        base(blockchainNodeClient, idBlockchainRepository, hotPoolRepository, localNodeContextProvider,
            blockchainNodesRepository)
    {
    }

    public override MessageTypes MessageType => MessageTypes.WitnessNodesList;
    protected override Message ProcessPayload(Message message)
    {
        if (message.Payload is WitnessNodesList payload)
        {
            BlockchainNodesRepository.ReplaceAllNodes(payload.Nodes);
            
            if (!payload.Nodes.Any(n => n.NodeId.Equals(LocalNodeContextProvider.LocalNodeId())))
            {
                BlockchainNodesRepository.AddOrUpdateNode(new BlockchainNode(LocalNodeContextProvider.LocalNodeId(),
                    LocalNodeContextProvider.IsWitnessNode, null, null, null, LocalNodeContextProvider.IsDnsNode));
            }
        }

        return null;
    }
}