using System;
using System.Collections.Generic;

namespace iDi.Blockchain.Framework.Execution
{
    /// <summary>
    /// Creates a new pipeline instance based on the provided pipeline stage types
    /// </summary>
    public class PipelineFactory
    {
        private readonly List<Type> _stages;
        private readonly IServiceProvider _serviceProvider;

        public PipelineFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _stages = new List<Type>();

        }

        /// <summary>
        /// Adds a new stage type the pipeline. The order in which the stages are added determines the execution order of the stages in the created pipeline.
        /// </summary>
        /// <typeparam name="TStage">Pipeline stage type to add</typeparam>
        public void AddStage<TStage>() where TStage : IPipelineStage
        {
            _stages.Add(typeof(TStage));
        }

        /// <summary>
        /// Creates a new pipeline instance
        /// </summary>
        /// <returns></returns>
        public Pipeline Create()
        {
            var stageInstances = new List<IPipelineStage>();
            foreach (var typ in _stages)
                stageInstances.Add((IPipelineStage) _serviceProvider.GetService(typ));

            return new Pipeline(stageInstances);
        }
    }
}
