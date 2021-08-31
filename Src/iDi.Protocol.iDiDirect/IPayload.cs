using iDi.Blockchain.Core.Messages;

namespace iDi.Protocol.iDiDirect
{
    public interface IPayload : IByteData
    {
        public MessageTypes MessageType { get;}
        public int Checksum { get;}
    }
}