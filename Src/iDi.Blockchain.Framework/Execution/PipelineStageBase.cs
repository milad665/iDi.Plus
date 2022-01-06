using System.Collections.ObjectModel;
using System.Net.Sockets;
using iDi.Blockchain.Framework.Communication;
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
        public abstract void HandleExecute(RequestContext request);

        /// <summary>
        /// Send message to all other nodes
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="forward">Determines if the message should only be forwarded to next nodes or not. If true, the message will not be sent to the node sending the current request.</param>
        protected void Broadcast(Message message, bool forward)
        {
            foreach (var nodeId in Nodes.Keys)
            {
                if (!forward || nodeId != _requestContext.Message.Header.NodeId)
                    SendMessage(nodeId, message);
            }
        }

        /// <summary>
        /// Send message to a specific node
        /// </summary>
        /// <param name="nodeId">Id of the receiver node</param>
        /// <param name="message">Message to send</param>
        /// <returns>Response Message</returns>
        protected Message SendToNode(string nodeId, Message message)
        {
            return SendMessage(nodeId, message);
        }

        /// <summary>
        /// Blockchain nodes list
        /// </summary>
        public ReadOnlyDictionary<string, BlockchainNode> Nodes { get; set; }

        /// <summary>
        /// Stage response, if not null, the pipeline will abort execution of next stages and returns the response.
        /// </summary>
        public abstract Message Response { get; protected set; }

        private Message SendMessage(string nodeId, Message message)
        {
            var node = Nodes[nodeId];
            if (node == null)
                return null;

            return BlockchainNodeClient.Send(node.VerifiedEndpoint1, message);
        }
    }
}
