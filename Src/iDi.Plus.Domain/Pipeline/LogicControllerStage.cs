using System.Collections;
using System.ComponentModel.DataAnnotations;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Blockchain;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Execution;
using iDi.Blockchain.Framework.Protocol;
using Org.BouncyCastle.Asn1.Cmp;

namespace iDi.Plus.Domain.Pipeline;

public class LogicControllerStage : PipelineStageBase
{
    private readonly IBlockchainRepository _blockchainRepository;

    public LogicControllerStage(IBlockchainNodeClient blockchainNodeClient, IBlockchainRepository blockchainRepository)
        :base(blockchainNodeClient)
    {
        _blockchainRepository = blockchainRepository;
    }

    public override void HandleExecute(RequestContext request)
    {
        var message = request.Message;
        var result = message.Process(request.LocalNodeId, request.LocalKeys.PrivateKey, _blockchainRepository);
        while (result.MessageToSend != null && result.TransmissionType != MessageTransmissionTypes.None)
        {
            switch (result.TransmissionType)
            {
                case MessageTransmissionTypes.Reply:
                    var response = SendToNode(message.Header.NodeId, result.MessageToSend);
                    if (response == null)
                        return;
                    
                    result = response.Process(request.LocalNodeId, request.LocalKeys.PrivateKey, _blockchainRepository);
                    break;
                case MessageTransmissionTypes.Broadcast:
                    Broadcast(result.MessageToSend, false);
                    break;
                case MessageTransmissionTypes.Forward:
                    Broadcast(result.MessageToSend, true);
                    break;
            }
        }
    }

    public override Message Response { get; protected set; }
}