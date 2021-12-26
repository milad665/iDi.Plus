using iDi.Blockchain.Framework.Protocol.iDiDirect;

namespace iDi.Blockchain.Framework.Execution
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
        Message Response { get; }
    }
}
