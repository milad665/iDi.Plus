using iDi.Blockchain.Core;
using iDi.Blockchain.Core.Messages;
using System;

namespace iDi.Protocol.iDiDirect.Payloads
{
    public class PayloadFactory
    {
        public IPayload CreatePayload(Networks network, short version, MessageTypes messageType, byte[] messageData)
        {
            if (network != Networks.Main || version != 1)
                throw new NotSupportedException("Network or version not supported");

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
