using iDi.Blockchain.Framework.Protocol.iDiDirect;

namespace iDi.Blockchain.Framework.Execution
{
    /// <summary>
    /// Handle the logic execution stages upon the arrival of a message
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        /// Execute the pipeline on a request
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Response message</returns>
        Message Execute(RequestContext request);
    }
}
