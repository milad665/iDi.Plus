using System.Collections.ObjectModel;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Blockchain.Framework.Execution
{
    public abstract class PipelineStageBase : IPipelineStage
    {
        private RequestContext _requestContext;
        protected readonly IBlockchainNodeClient BlockchainNodeClient;

        protected PipelineStageBase(IBlockchainNodeClient blockchainNodeClient)
        {
            BlockchainNodeClient = blockchainNodeClient;
        }

        /// <summary>
        /// Executes the stage on the request
        /// </summary>
        /// <param name="request"></param>
        public void Execute(RequestContext request)
        {
            _requestContext = request;
            HandleExecute(_requestContext);
        }

        /// <summary>
        /// When implemented, this method handles the execution logic of the pipeline stage.
        /// </summary>
        /// <param name="request">Incoming request</param>
        protected abstract void HandleExecute(RequestContext request);

        /// <summary>
        /// Blockchain nodes list
        /// </summary>
        public ReadOnlyDictionary<NodeIdValue, BlockchainNode> Nodes { get; set; }

        /// <summary>
        /// Stage response, if not null, the pipeline will abort execution of next stages and returns the response.
        /// </summary>
        public abstract Message Response { get; protected set; }
    }
}
