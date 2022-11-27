using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

public class WitnessNodesListPayload : MainNetworkV1PayloadBase
{
    public WitnessNodesListPayload(byte[] rawData) : base(rawData, MessageTypes.WitnessNodesList)
    {
        ExtractRawData(rawData);
    }

    private WitnessNodesListPayload(byte[] rawData, IEnumerable<BlockchainNode> nodes) : base(rawData, MessageTypes.WitnessNodesList)
    {
        Nodes = nodes;
    }

    public static WitnessNodesListPayload Create(IEnumerable<BlockchainNode> nodes)
    {
        var bytes = new List<byte>();
        foreach (var node in nodes)
        {
            var isEp1V6 = node.VerifiedEndpoint1?.Address != null && node.VerifiedEndpoint1.AddressFamily == AddressFamily.InterNetworkV6;
            var isEp2V6 = node.VerifiedEndpoint2?.Address != null && node.VerifiedEndpoint2.AddressFamily == AddressFamily.InterNetworkV6;
            var ep1IpBytes = new byte[4];
            var ep2IpBytes = new byte[4];
            var ep1Port = 0;
            var ep2Port = 0;
            if (node.VerifiedEndpoint1?.Address != null)
            {
                ep1IpBytes = node.VerifiedEndpoint1.Address.GetAddressBytes();
                ep1Port = node.VerifiedEndpoint1.Port;
            }

            if (node.VerifiedEndpoint2?.Address != null)
            {
                ep2IpBytes = node.VerifiedEndpoint2.Address.GetAddressBytes();
                ep2Port = node.VerifiedEndpoint2.Port;
            }

            bytes.AddRange(node.NodeId.Bytes); //NodeIdValue.NodeIdByteLength = 91 bytes
            bytes.Add(Convert.ToByte(isEp1V6));//1
            bytes.AddRange(ep1IpBytes);//4 or 16
            bytes.AddRange(BitConverter.GetBytes((short)ep1Port)); //2
            bytes.Add(Convert.ToByte(isEp2V6)); //1
            bytes.AddRange(ep2IpBytes); //4 or 16
            bytes.AddRange(BitConverter.GetBytes((short)ep2Port)); //2
            bytes.Add(Convert.ToByte(node.IsDns)); //1
            bytes.Add(Convert.ToByte(node.IsWitnessNode)); //1
        }

        return new WitnessNodesListPayload(bytes.ToArray(), nodes);
    }

    public IEnumerable<BlockchainNode> Nodes { get; private set; }

    private void ExtractRawData(byte[] rawData)
    {
        var nodes = new List<BlockchainNode>();

        var span = new ReadOnlySpan<byte>(rawData);
        var index = 0;
        while (index < rawData.Length)
        {
            //NodeId
            var size = NodeIdValue.NodeIdByteLength;
            var nodeId = new NodeIdValue(span.Slice(index, size).ToArray());
            index += size;

            //Verified EP 1
            size = 1;
            var isEp1V6 = Convert.ToBoolean(span.Slice(index, size).ToArray()[0]);
            index += size;
            size = isEp1V6 ? 16 : 4;
            var ep1IpBytes = span.Slice(index, size).ToArray();
            index += size;
            var ep1IpAddress = ep1IpBytes.All(b => b == 0) ? null : new IPAddress(ep1IpBytes);
            size = 2;
            var ep1Port = BitConverter.ToInt16(span.Slice(index, size).ToArray());
            index += size;
            var verifiedEp1 = ep1IpAddress == null ? null : new IPEndPoint(ep1IpAddress, ep1Port);

            //Verified EP 2
            size = 1;
            var isEp2V6 = Convert.ToBoolean(span.Slice(index, size).ToArray()[0]);
            index += size;
            size = isEp2V6 ? 16 : 4;
            var ep2IpBytes = span.Slice(index, size).ToArray();
            index += size;
            var ep2IpAddress = ep2IpBytes.All(b => b == 0) ? null : new IPAddress(ep2IpBytes);
            size = 2;
            var ep2Port = BitConverter.ToInt16(span.Slice(index, size).ToArray());
            index += size;
            var verifiedEp2 = ep2IpAddress == null ? null : new IPEndPoint(ep2IpAddress, ep2Port);

            //IsDns
            size = 1;
            var isDns = Convert.ToBoolean(span.Slice(index, size).ToArray()[0]);
            index += size;

            //IsWitnessNode
            size = 1;
            var isWitnessNode = Convert.ToBoolean(span.Slice(index, size).ToArray()[0]);
            index += size;

            nodes.Add(new BlockchainNode(nodeId, isWitnessNode, verifiedEp1, verifiedEp2, null, isDns));
        }

        Nodes = nodes;
    }
}