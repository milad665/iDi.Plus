using System.Collections.Generic;
using System.Collections.ObjectModel;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Blockchain.Framework.Providers;

public interface IBlockchainNodesProvider
{
    void AddOrUpdateNode(BlockchainNode node);
    void AddOrUpdateNodeRange(IEnumerable<BlockchainNode> nodes);
    ReadOnlyDictionary<string, BlockchainNode> ToDictionary();
    IEnumerable<string> AllNodeIds();
    IEnumerable<BlockchainNode> AllNodes();
    BlockchainNode this[string nodeId] { get; }
}