namespace iDi.Blockchain.Framework.Protocol.iDiDirect
{
    public interface IPayload : IByteData
    {
        public MessageTypes MessageType { get;}
        public short Version { get;}
        public Networks Network { get;}
        public int Checksum { get;}
    }
}