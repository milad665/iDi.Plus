using System;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

public class GetWitnessNodesPayload : MainNetworkV1PayloadBase
{
    protected GetWitnessNodesPayload(byte[] rawData) : base(rawData, MessageTypes.GetWitnessNodes)
    {
    }

    public static GetWitnessNodesPayload Create()
    {
        return new GetWitnessNodesPayload(Array.Empty<byte>());
    }
}