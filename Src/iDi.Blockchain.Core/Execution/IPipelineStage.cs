using iDi.Blockchain.Core.Messages;

namespace iDi.Blockchain.Core.Execution
{
    public interface IPipelineStage
    {
        /// <summary>
        /// Executes the stage on the request
        /// </summary>
        /// <param name="request"></param>
        void Execute(RequestContext request);
        /// <summary>
        /// Stage response, if not null, the pipeline will abort execution of next stages and returns the response.
        /// </summary>
        IMessage Response { get; }
    }
}
