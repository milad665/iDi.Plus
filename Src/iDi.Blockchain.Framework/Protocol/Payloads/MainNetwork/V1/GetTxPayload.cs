﻿using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Protocol.Extensions;

namespace iDi.Blockchain.Framework.Protocol.Payloads.MainNetwork.V1
{
    /// <summary>
    /// Payload of GetTx (Get Transaction) request message
    /// </summary>
    public class GetTxPayload : MainNetworkV1PayloadBase
    {
        public GetTxPayload(byte[] rawData) : base(rawData, MessageTypes.GetTx)
        {
            TransactionHash = rawData.ToHexString();
        }

        public string TransactionHash { get; set; }
        public override (IPayload PayloadToSend, MessageTransmissionTypes TransmissionType) Process(IBlockchainRepository blockchainRepository)
        {
            throw new System.NotImplementedException();
        }
    }
}