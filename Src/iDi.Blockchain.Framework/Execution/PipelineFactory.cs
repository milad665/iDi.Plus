using System;
using System.Collections.Generic;
using iDi.Blockchain.Framework.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace iDi.Blockchain.Framework.Execution;

/// <summary>
/// Creates a new pipeline instance based on the provided pipeline stage types
/// </summary>
public class PipelineFactory : IPipelineFactory
{
    private readonly List<Type> _stages;
    private readonly IServiceProvider _serviceProvider;
    private readonly BlockchainNodesProvider _blockchainNodesProvider;

    public PipelineFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _blockchainNodesProvider = _serviceProvider.GetService<BlockchainNodesProvider>();

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
    public IPipeline Create()
    {
        var stageInstances = new List<IPipelineStage>();
        using var scope = _serviceProvider.CreateScope();

        foreach (var typ in _stages)
        {
            var pipelineStage = (IPipelineStage) scope.ServiceProvider.GetService(typ);
            if (pipelineStage == null)
                throw new Exception($"Unable to create pipeline stage instance of type '{typ}'");

            pipelineStage.Nodes = _blockchainNodesProvider.ToDictionary();
            stageInstances.Add(pipelineStage);
        }

        return new Pipeline(stageInstances);
    }
}