using System;
using System.Collections.Generic;
using iDi.Blockchain.Framework.Messages;
using iDi.Protocol.iDiDirect.Payloads;

namespace iDi.Protocol.iDiDirect
{
    /// <summary>
    /// The object representation of a message transmitted between nodes over the network.
    /// </summary>
    public class Message : IMessage
    {
        protected Message(Header header, IPayload payload, byte[] rawData)
        {
            Header = header;
            Payload = payload;
            RawData = rawData;
        }

        /// <summary>
        /// Creates a new message instance from input byte data.
        /// This method is mainly used to reconstruct the message object from the data received over the network.
        /// </summary>
        /// <param name="data">Raw byte data</param>
        /// <returns>Message instance</returns>
        public static Message FromMessageData(ReadOnlySpan<byte> data)
        {
            var header = Header.FromPacketData(data);
            var payloadFactory = new PayloadFactory();
            var payload = payloadFactory.CreatePayload(header.Network, header.Version, header.MessageType, data.Slice(header.RawData.Length).ToArray());

            return new Message(header, payload, data.ToArray());
        }

        /// <summary>
        /// Creates a new message instance from header and payload objects.
        /// This method is mainly used to create a new message before sending it to the network
        /// </summary>
        /// <param name="header">Header instance</param>
        /// <param name="payload">payload instance</param>
        /// <returns>Message instance</returns>
        public static Message Create(Header header, IPayload payload)
        {
            var lstData = new List<byte>();
            lstData.AddRange(header.RawData);
            lstData.AddRange(payload.RawData);

            return new Message(header, payload, lstData.ToArray());
        }

        /// <summary>
        /// Message header
        /// </summary>
        public Header Header { get; private set; }
        /// <summary>
        /// Message payload
        /// </summary>
        public IPayload Payload { get; private set; }
        /// <summary>
        /// Raw byte data of the whole message
        /// </summary>
        public byte[] RawData { get; }
    }
}