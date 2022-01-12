namespace iDi.Blockchain.Framework.Execution;

public interface IPipelineFactory
{
    /// <summary>
    /// Adds a new stage type the pipeline. The order in which the stages are added determines the execution order of the stages in the created pipeline.
    /// </summary>
    /// <typeparam name="TStage">Pipeline stage type to add</typeparam>
    void AddStage<TStage>() where TStage : IPipelineStage;

    /// <summary>
    /// Creates a new pipeline instance
    /// </summary>
    /// <returns></returns>
    IPipeline Create();
}