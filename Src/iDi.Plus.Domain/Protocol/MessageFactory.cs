using System;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Extensions;

namespace iDi.Plus.Domain.Protocol
{
    public class MessageFactory : IMessageFactory
    {
        public Message CreateMessage(ReadOnlySpan<byte> messageData)
        {
            var header = Header.FromPacketData(messageData);
            var payload = CreatePayload(header.Network, header.Version, header.MessageType,
                messageData.Slice(header.RawData.Length).ToArray(), header.PayloadSignature,
                header.NodeId.HexStringToByteArray());

            return new Message(header, payload, messageData.ToArray());
        }

        private IPayload CreatePayload(Networks network, short version, MessageTypes messageType, byte[] messageData, byte[] messageSignature, byte[] nodeId)
        {
            var cryptoServiceProvider = new CryptoServiceProvider();

            if (network != Networks.Main || version != 1)
                throw new NotSupportedException("Network or version not supported");

            if (!cryptoServiceProvider.Verify(nodeId, messageData, messageSignature))
                throw new UnauthorizedAccessException("Unable to verify message signature. BlockchainNode Id (node public key) does not match its real origin.");

            switch (messageType)
            {
                case MessageTypes.NewTxs:
                    return new Payloads.MainNetwork.V1.NewTxsPayload(messageData);
                case MessageTypes.GetTx:
                    return new Payloads.MainNetwork.V1.GetTxPayload(messageData);
                case MessageTypes.TxData:
                    return new Payloads.MainNetwork.V1.TxDataPayload(messageData);
                case MessageTypes.NewBlocks:
                    return new Payloads.MainNetwork.V1.NewBlocksPayload(messageData);
                case MessageTypes.GetBlock:
                    return new Payloads.MainNetwork.V1.GetBlockPayload(messageData);
                case MessageTypes.BlockData:
                    return new Payloads.MainNetwork.V1.BlockDataPayload(messageData);
                case MessageTypes.CreateTx:
                    return new Payloads.MainNetwork.V1.CreateTxPayload(messageData);
                case MessageTypes.Empty:
                    return null;
                default:
                    throw new NotSupportedException("Message type not supported");
            }
        }
    }
}
