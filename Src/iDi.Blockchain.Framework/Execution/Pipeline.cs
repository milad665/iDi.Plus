using iDi.Blockchain.Framework.Exceptions;
using System.Collections.Generic;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Blockchain.Framework.Execution
{
    public class Pipeline : IPipeline
    {
        private readonly List<IPipelineStage> _stages;

        public Pipeline(List<IPipelineStage> stages)
        {
            _stages = stages;
        }

        public Message Execute(RequestContext request)
        {
            Message response = null;

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
