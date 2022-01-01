using iDi.Blockchain.Framework.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Blockchain.Framework.Execution
{
    public abstract class PipelineStageBase : IPipelineStage
    {
        private RequestContext _requestContext;

        protected PipelineStageBase(Dictionary<Guid, Node> dicNodes)
        {
            Nodes = new ReadOnlyDictionary<Guid, Node>(dicNodes);
        }

        /// <summary>
        /// Executes the stage on the request
        /// </summary>
        /// <param name="request"></param>
        public abstract void Execute(RequestContext request);

        /// <summary>
        /// Send message to all, except the one that the message was received from
        /// </summary>
        /// <param name="message">Message to send</param>
        protected void ForwardToAll(Message message)
        {
            foreach (var nodeId in Nodes.Keys)
            {
                if (nodeId != _requestContext.Message.Header.NodeId)
                    SendMessage(nodeId, message);
            }
        }

        /// <summary>
        /// Send message to a specific node
        /// </summary>
        /// <param name="nodeId">Id of the receiver node</param>
        /// <param name="message">Message to send</param>
        protected void SendToNode(Guid nodeId, Message message)
        {
            SendMessage(nodeId, message);
        }

        protected ReadOnlyDictionary<Guid, Node> Nodes { get; }

        /// <summary>
        /// Stage response, if not null, the pipeline will abort execution of next stages and returns the response.
        /// </summary>
        public abstract Message Response { get; protected set; }

        private void SendMessage(Guid nodeId, Message message)
        {
            var node = Nodes[nodeId];
            if (node == null)
                return;

            var tcpClient = new TcpClient();
            
            //TODO: Fallback to VerifiedEndpoint2 must be implemented
            tcpClient.Connect(node.VerifiedEndpoint1);

            var stream = tcpClient.GetStream();
            stream.Write(message.RawData);
            stream.Close();
            tcpClient.Close();
        }
    }
}
