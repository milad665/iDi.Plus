using System;

namespace iDi.Blockchain.Framework.Protocol
{
    /// <summary>
    /// Contains data extracted from the 32-byte Packed Header
    /// </summary>
    public class Header : IByteData
    {
        protected Header(Networks network, short version, Guid nodeId, MessageTypes messageType, int payloadLength, int payloadChecksum, byte[] rawData)
        {
            Network = network;
            Version = version;
            NodeId = nodeId;
            MessageType = messageType;
            PayloadLength = payloadLength;
            PayloadChecksum = payloadChecksum;
            RawData = rawData;
        }

        public static Header Create(Networks network, short version, Guid nodeId, MessageTypes messageType, IPayload payload)
        {
            var result = new byte[32];

            var networkBytes = BitConverter.GetBytes((int)network);
            var versionBytes = BitConverter.GetBytes(version);
            var nodeIdBytes = nodeId.ToByteArray();
            var messageTypeBytes = BitConverter.GetBytes((int)messageType);
            var payloadLengthBytes = BitConverter.GetBytes(payload.RawData.Length);
            var payloadChecksumBytes = BitConverter.GetBytes(payload.Checksum);

            var index = 0;
            Array.Copy(networkBytes, result, networkBytes.Length);
            index += networkBytes.Length;
            Array.Copy(versionBytes,0, result, index, versionBytes.Length);
            index += versionBytes.Length;
            Array.Copy(nodeIdBytes, 0, result, index, nodeIdBytes.Length);
            index += nodeIdBytes.Length;
            Array.Copy(messageTypeBytes, 0, result, index, messageTypeBytes.Length);
            index += messageTypeBytes.Length;
            Array.Copy(payloadLengthBytes, 0, result, index, payloadLengthBytes.Length);
            index += payloadLengthBytes.Length;
            Array.Copy(payloadChecksumBytes, 0, result, index, payloadChecksumBytes.Length);

            return new Header(network, version, nodeId, messageType, payload.RawData.Length, payload.Checksum, result);
        }

        public static Header FromPacketData(ReadOnlySpan<byte> data)
        {
            var rawData = data.Slice(0, 32);
            var index = 0;

            var network = BitConverter.ToInt32(rawData.Slice(index, 4));
            index += 4;
            var version = BitConverter.ToInt16(rawData.Slice(index, 2));
            index += 2;
            var nodeId = new Guid(rawData.Slice(index, 16));
            index += 16;
            var messageType = (int)rawData.Slice(16, 1)[0];
            index++;
            var payloadLength = BitConverter.ToInt32(rawData.Slice(index, 4));
            index += 4;
            var payloadChecksum = BitConverter.ToInt32(rawData.Slice(index, 4));
            return new Header((Networks)network, version, nodeId, (MessageTypes) messageType, payloadLength, payloadChecksum,
                rawData.ToArray());
        }

        public Networks Network { get; }
        public short Version { get; }
        public Guid NodeId { get; }
        public MessageTypes MessageType { get; }
        public int PayloadLength { get; }
        public int PayloadChecksum { get; }
        public byte[] RawData { get; }
    }
}