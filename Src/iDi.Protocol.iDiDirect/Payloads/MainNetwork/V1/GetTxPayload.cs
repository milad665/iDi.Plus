using System.Text;
using iDi.Blockchain.Core;
using iDi.Blockchain.Core.Messages;
using iDi.Protocol.iDiDirect.Exceptions;

namespace iDi.Protocol.iDiDirect.Payloads.MainNetwork.V1
{
    public class GetTxPayload : PayloadBase
    {
        public GetTxPayload(byte[] rawData) : base(rawData, MessageTypes.GetTx)
        {
            TransactionHash = GetTransactionHash(rawData);
        }

        public string TransactionHash { get; set; }

        private string GetTransactionHash(byte[] rawData)
        {
            var txHashByteLength = Cryptography.HashAlgorithm.HashSize / 8;

            if (rawData.Length != txHashByteLength)
                throw new InvalidDataException("Data length does not match the hash length.");

            return Encoding.ASCII.GetString(rawData);
        }
    }
}