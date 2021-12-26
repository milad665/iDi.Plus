using iDi.Blockchain.Core.Exceptions;
using iDi.Blockchain.Core.Messages;
using System.Collections.Generic;

namespace iDi.Blockchain.Core.Execution
{
    public class Pipeline : IPipeline
    {
        private readonly List<IPipelineStage> _stages;

        public Pipeline()
        {
            _stages = new List<IPipelineStage>();
        }

        public void AddStage(IPipelineStage stage)
        {
            _stages.Add(stage);
        }

        public IMessage Execute(RequestContext request)
        {
            IMessage response = null;

            foreach (var stage in _stages)
            {
                stage.Execute(request);
                response = stage.Response;
                if (response != null)
                    break;
            }

            if (response == null)
                throw new InvalidResponseException("Pipeline response can not be null.");

            return response;
        }
    }
}
