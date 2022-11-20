using System.Collections.Generic;
using System.Linq;
using iDi.Blockchain.Framework;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Execution;
using iDi.Blockchain.Framework.Protocol;
using iDi.Plus.Domain.Protocol.Processors;

namespace iDi.Plus.Domain.Pipeline;

public class LogicControllerStage : PipelineStageBase
{
    private readonly IEnumerable<IMessageProcessor> _messageProcessors;

    public LogicControllerStage(IBlockchainNodeClient blockchainNodeClient, IEnumerable<IMessageProcessor> messageProcessors)
        :base(blockchainNodeClient)
    {
        _messageProcessors = messageProcessors;
    }

    /// <inheritdoc/>
    protected override void HandleExecute(RequestContext request)
    {
        var message = request.Message;
        var processor = _messageProcessors.FirstOrDefault(p => p.CanProcess(message));
        if (processor == null)
            return;

        Response = processor.Process(message);
    }
}