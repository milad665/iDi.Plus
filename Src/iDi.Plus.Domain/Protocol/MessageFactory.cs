using System;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Exceptions;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Plus.Domain.Protocol
{
    public class MessageFactory : IMessageFactory
    {
        public Message CreateMessage(ReadOnlySpan<byte> messageData)
        {
            var header = Header.FromPacketData(messageData);
            var payload = CreatePayload(header.Network, header.Version, header.MessageType,
                messageData.Slice(header.RawData.Length).ToArray(), header.PayloadSignature,
                header.NodeId.Bytes);

            return new Message(header, payload, messageData.ToArray());
        }

        private IPayload CreatePayload(Networks network, short version, MessageTypes messageType, byte[] payloadData, byte[] messageSignature, byte[] nodeId)
        {
            var cryptoServiceProvider = new CryptoServiceProvider();

            if (network != Networks.Main || version != 1)
                throw new NotSupportedException("Network or version not supported");

            if (!cryptoServiceProvider.Verify(nodeId, payloadData, messageSignature))
                throw new UnauthorizedException("Unable to verify message signature. BlockchainNode Id (node public key) does not match its real origin.");

            switch (messageType)
            {
                case MessageTypes.BlockData:
                    return new Payloads.MainNetwork.V1.BlockDataPayload(payloadData);
                case MessageTypes.CreateTx:
                    return new Payloads.MainNetwork.V1.CreateTxPayload(payloadData);
                case MessageTypes.GetBlock:
                    return new Payloads.MainNetwork.V1.GetBlockPayload(payloadData);
                case MessageTypes.GetNewBlocks:
                    return new Payloads.MainNetwork.V1.GetNewBlocksPayload(payloadData);
                case MessageTypes.GetTx:
                    return new Payloads.MainNetwork.V1.GetTxPayload(payloadData);
                case MessageTypes.GetWitnessNodes:
                    return new Payloads.MainNetwork.V1.GetWitnessNodesPayload(payloadData);
                case MessageTypes.NewBlocks:
                    return new Payloads.MainNetwork.V1.NewBlocksPayload(payloadData);
                case MessageTypes.NewTxs:
                    return new Payloads.MainNetwork.V1.NewTxsPayload(payloadData);
                case MessageTypes.TxDataRequest:
                    return new Payloads.MainNetwork.V1.TxDataRequestPayload(payloadData);
                case MessageTypes.TxDataResponse:
                    return new Payloads.MainNetwork.V1.TxDataResponsePayload(payloadData);
                case MessageTypes.Vote:
                    return new Payloads.MainNetwork.V1.VotePayload(payloadData);
                case MessageTypes.WitnessNodesList:
                    return new Payloads.MainNetwork.V1.WitnessNodesListPayload(payloadData);
                case MessageTypes.Empty:
                    return null;
                default:
                    throw new NotSupportedException("Message type not supported");
            }
        }
    }
}
