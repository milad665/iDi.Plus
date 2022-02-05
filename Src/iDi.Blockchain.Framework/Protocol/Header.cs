using System;
using System.Collections.Generic;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol.Extensions;

namespace iDi.Blockchain.Framework.Protocol
{
    /// <summary>
    /// Contains data extracted from the Packed Header
    /// </summary>
    public class Header : IByteData
    {
        protected Header(Networks network, short version, NodeIdValue nodeId, MessageTypes messageType, int payloadLength, byte[] payloadSignature, byte[] rawData)
        {
            Network = network;
            Version = version;
            NodeId = nodeId;
            MessageType = messageType;
            PayloadLength = payloadLength;
            PayloadSignature = payloadSignature;
            RawData = rawData;
        }

        public static Header Create(Networks network, short version, NodeIdValue nodeId, MessageTypes messageType, int payloadSize, byte[] payloadSignature)
        {
            var bytes = new List<byte>();

            var networkBytes = BitConverter.GetBytes((int)network);
            var versionBytes = BitConverter.GetBytes(version);
            var nodeIdBytes = nodeId.Bytes;
            var payloadLengthBytes = BitConverter.GetBytes(payloadSize);

            bytes.AddRange(networkBytes);
            bytes.AddRange(versionBytes);
            bytes.AddRange(nodeIdBytes);
            bytes.Add((byte)messageType);
            bytes.AddRange(payloadLengthBytes);
            bytes.AddRange(payloadSignature);
            var headerLengthBytes = BitConverter.GetBytes(bytes.Count);
            var result = new List<byte>();
            result.AddRange(headerLengthBytes);
            result.AddRange(bytes);

            return new Header(network, version, nodeId, messageType, payloadSize, payloadSignature,result.ToArray());
        }

        public static Header FromPacketData(ReadOnlySpan<byte> data)
        {
            var length = BitConverter.ToInt32(data.Slice(0,4));

            var rawData = data.Slice(0, length + 4);
            var index = 4;

            var network = BitConverter.ToInt32(rawData.Slice(index, 4));
            index += 4;
            var version = BitConverter.ToInt16(rawData.Slice(index, 2));
            index += 2;
            var nodeId = rawData.Slice(index, DigitalSignatureKeys.PublicKeyByteLength).ToArray();
            index += NodeIdValue.NodeIdByteLength;
            var messageType = rawData.Slice(index, 1)[0];
            index++;
            var payloadLength = BitConverter.ToInt32(rawData.Slice(index, 4));
            index += 4;
            var payloadSignature = rawData.Slice(index);
            return new Header((Networks) network, version, new NodeIdValue(nodeId), (MessageTypes) messageType, payloadLength,
                payloadSignature.ToArray(), rawData.ToArray());
        }

        public Header ToResponseHeader(NodeIdValue senderNodeId, MessageTypes messageType, int payloadSize, byte[] payloadSignature)
        {
            return Create(Network, Version, senderNodeId, messageType, payloadSize, payloadSignature);
        }

        public Networks Network { get; }
        public short Version { get; }
        public NodeIdValue NodeId { get; }
        public MessageTypes MessageType { get; }
        public int PayloadLength { get; }
        public byte[] PayloadSignature { get; }
        public byte[] RawData { get; }
    }
}