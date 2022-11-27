using System;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Protocol.Exceptions;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

public class GetWitnessNodesPayload : MainNetworkV1PayloadBase
{
    public GetWitnessNodesPayload(byte[] rawData) : base(rawData ?? Array.Empty<byte>(), MessageTypes.GetWitnessNodes)
    {
        ExtractData(rawData);
    }

    public static GetWitnessNodesPayload Create()
    {
        return new GetWitnessNodesPayload(Array.Empty<byte>());
    }

    private void ExtractData(byte[] rawData)
    {
        if (rawData == null)
            return;
        
        if (rawData.Length>0)
            throw new InvalidInputException("Byte array must be empty.");
    }
}