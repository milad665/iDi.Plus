using System;
using iDi.Blockchain.Framework.Cryptography;

namespace iDi.Blockchain.Framework.Protocol.Payloads
{
    public class PayloadFactory
    {
        /// <summary>
        /// Creates a new payload instance based of the message type.
        /// Before creating the instance, the message origin is verified using node Id and the message signature.
        /// </summary>
        /// <param name="network"></param>
        /// <param name="version"></param>
        /// <param name="messageType"></param>
        /// <param name="messageData"></param>
        /// <param name="messageSignature"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public IPayload CreatePayload(Networks network, short version, MessageTypes messageType, byte[] messageData, byte[] messageSignature, byte[] nodeId)
        {
            var cryptoServiceProvider = new CryptoServiceProvider();

            if (network != Networks.Main || version != 1)
                throw new NotSupportedException("Network or version not supported");

            if (!cryptoServiceProvider.Verify(nodeId, messageData, messageSignature))
                throw new UnauthorizedAccessException("Unable to verify message signature. Node Id (node public key) does not match its real origin.");

            switch (messageType)
            {
                case MessageTypes.NewTxs:
                    return new MainNetwork.V1.NewTxsPayload(messageData);
                case MessageTypes.GetTx:
                    return new MainNetwork.V1.GetTxPayload(messageData);
                case MessageTypes.TxData:
                    return new MainNetwork.V1.TxDataPayload(messageData);
                case MessageTypes.NewBlocks:
                    return new MainNetwork.V1.NewBlocksPayload(messageData);
                case MessageTypes.GetBlock:
                    return new MainNetwork.V1.GetBlockPayload(messageData);
                case MessageTypes.BlockData:
                    return new MainNetwork.V1.BlockDataPayload(messageData);
                case MessageTypes.CreateTx:
                    return new MainNetwork.V1.CreateTxPayload(messageData);
                case MessageTypes.Empty:
                default:
                    throw new NotSupportedException("Message type not supported");
            }
        }
    }
}
