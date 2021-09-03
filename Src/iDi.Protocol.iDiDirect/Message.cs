using System;
using System.Collections.Generic;
using iDi.Blockchain.Core.Messages;
using iDi.Protocol.iDiDirect.Payloads.MainNetwork.V1;

namespace iDi.Protocol.iDiDirect
{
    public class Message : IByteData
    {
        protected Message(Header header, IPayload payload, byte[] rawData)
        {
            Header = header;
            Payload = payload;
            RawData = rawData;
        }

        public static Message FromMessageData(ReadOnlySpan<byte> data)
        {
            var header = Header.FromPacketData(data);
            var payload = GetPayload(header.MessageType, data.Slice(header.RawData.Length));

            return new Message(header, payload, data.ToArray());
        }

        public static Message Create(Header header, IPayload payload)
        {
            var lstData = new List<byte>();
            lstData.AddRange(header.RawData);
            lstData.AddRange(payload.RawData);

            return new Message(header, payload, lstData.ToArray());
        }

        public Header Header { get; private set; }
        public IPayload Payload { get; private set; }
        public byte[] RawData { get; }

        private static IPayload GetPayload(MessageTypes messageType, ReadOnlySpan<byte> messageData)
        {
            switch (messageType)
            {
                case MessageTypes.NewTxs:
                    return new NewTxsPayload(messageData.ToArray());
                case MessageTypes.GetTx:
                    return new GetTxPayload(messageData.ToArray());
                case MessageTypes.TxData:
                    return new TxDataPayload(messageData.ToArray());
                case MessageTypes.NewBlocks:
                    return new NewBlocksPayload(messageData.ToArray());
                case MessageTypes.GetBlock:
                    return new GetBlockPayload(messageData.ToArray());
                case MessageTypes.BlockData:
                    return new BlockDataPayload(messageData.ToArray());
                default:
                    throw new NotSupportedException("Message type not supported");
            }
        }
    }
}