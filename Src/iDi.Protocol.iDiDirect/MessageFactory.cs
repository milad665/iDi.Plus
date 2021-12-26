using iDi.Blockchain.Framework.Messages;
using System;

namespace iDi.Protocol.iDiDirect
{
    public class MessageFactory : IMessageFactory
    {
        public IMessage FromMessageData(ReadOnlySpan<byte> data)
        {
            return Message.FromMessageData(data);
        }
    }
}
