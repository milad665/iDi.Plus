namespace iDi.Blockchain.Framework.Protocol
{
    public interface IPayload : IByteData
    {
        /// <summary>
        /// Type of the message
        /// </summary>
        public MessageTypes MessageType { get;}
        /// <summary>
        /// Protocol Version
        /// </summary>
        public short Version { get;}
        /// <summary>
        /// Blockchain Network
        /// </summary>
        public Networks Network { get;}
        /// <summary>
        /// Payload Checksum
        /// </summary>
        public int Checksum { get;}
    }
}