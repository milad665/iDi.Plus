using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1
{
    /// <summary>
    /// Payload of GetTx (GetKeys Transaction) request message
    /// </summary>
    public class GetTxPayload : MainNetworkV1PayloadBase
    {
        public GetTxPayload(byte[] rawData) : base(rawData, MessageTypes.GetTx)
        {
            TransactionHash = new HashValue(rawData);
        }

        protected GetTxPayload(byte[] rawData, HashValue transactionHash) : base(rawData, MessageTypes.GetTx)
        {
            TransactionHash = transactionHash;
        }

        public static GetTxPayload Create(HashValue transactionHash)
        {
            return new GetTxPayload(transactionHash.Bytes, transactionHash);
        }

        public HashValue TransactionHash { get; set; }
    }
}