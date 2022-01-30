using System;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1
{
    /// <summary>
    /// Payload of CreateTx (Create Transaction) command
    /// </summary>
    public class CreateTxPayload : TxDataPayload
    {
        public CreateTxPayload(byte[] rawData) : base(rawData, MessageTypes.CreateTx)
        {
        }

        public new static TxDataPayload Create(HashValue transactionHash, TransactionTypes transactionType,
            string issuerAddress, string holderAddress, string verifierAddress, string subject, string identifierKey,
            DateTime timestamp, HashValue previousTransactionHash, byte[] doubleEncryptedData)
        {
            return InternalCreate(transactionHash, transactionType, issuerAddress, holderAddress, verifierAddress,
                subject, identifierKey, timestamp, previousTransactionHash, doubleEncryptedData, MessageTypes.CreateTx);
        }
    }
}