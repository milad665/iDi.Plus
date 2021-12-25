using System;
using System.Collections.Generic;
using iDi.Blockchain.Core.Messages;
using iDi.Protocol.iDiDirect.Payloads;
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
            var payloadFactory = new PayloadFactory();
            var payload = payloadFactory.CreatePayload(header.Network, header.Version, header.MessageType, data.Slice(header.RawData.Length).ToArray());

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
    }
}