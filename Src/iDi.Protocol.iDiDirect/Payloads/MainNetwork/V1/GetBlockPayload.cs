using System.Text;
using iDi.Blockchain.Core;
using iDi.Blockchain.Core.Messages;
using iDi.Protocol.iDiDirect.Exceptions;

namespace iDi.Protocol.iDiDirect.Payloads.MainNetwork.V1
{
    public class GetBlockPayload : MainNetworkV1PayloadBase
    {
        public GetBlockPayload(byte[] rawData) : base(rawData, MessageTypes.GetBlock)
        {
            BlockHash = GetBlockHash(rawData);
        }

        public string BlockHash { get; set; }

        private string GetBlockHash(byte[] rawData)
        {
            var txHashByteLength = Cryptography.HashAlgorithm.HashSize / 8;

            if (rawData.Length != txHashByteLength)
                throw new InvalidDataException("Data length does not match the hash length.");

            return Encoding.ASCII.GetString(rawData);
        }
    }
}