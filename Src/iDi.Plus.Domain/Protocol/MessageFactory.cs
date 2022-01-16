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

        private IPayload CreatePayload(Networks network, short version, MessageTypes messageType, byte[] payloadData, byte[] messageSignature, byte[] nodeId)
        {
            var cryptoServiceProvider = new CryptoServiceProvider();

            if (network != Networks.Main || version != 1)
                throw new NotSupportedException("Network or version not supported");

            if (!cryptoServiceProvider.Verify(nodeId, payloadData, messageSignature))
                throw new UnauthorizedAccessException("Unable to verify message signature. BlockchainNode Id (node public key) does not match its real origin.");

            switch (messageType)
            {
                case MessageTypes.NewTxs:
                    return new Payloads.MainNetwork.V1.NewTxsPayload(payloadData);
                case MessageTypes.GetTx:
                    return new Payloads.MainNetwork.V1.GetTxPayload(payloadData);
                case MessageTypes.TxData:
                    return new Payloads.MainNetwork.V1.TxDataPayload(payloadData);
                case MessageTypes.NewBlocks:
                    return new Payloads.MainNetwork.V1.NewBlocksPayload(payloadData);
                case MessageTypes.GetBlock:
                    return new Payloads.MainNetwork.V1.GetBlockPayload(payloadData);
                case MessageTypes.BlockData:
                    return new Payloads.MainNetwork.V1.BlockDataPayload(payloadData);
                case MessageTypes.CreateTx:
                    return new Payloads.MainNetwork.V1.CreateTxPayload(payloadData);
                case MessageTypes.Empty:
                    return null;
                default:
                    throw new NotSupportedException("Message type not supported");
            }
        }
    }
}
