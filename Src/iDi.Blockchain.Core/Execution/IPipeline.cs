using iDi.Blockchain.Core.Messages;

namespace iDi.Blockchain.Core.Execution
{
    /// <summary>
    /// Handle the logic execution stages upon the arrival of a message
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        /// Adds a new stage the pipeline. The order in which the stages are added determines the execution order of the stages.
        /// </summary>
        /// <param name="stage">Pipeline stage to add</param>
        void AddStage(IPipelineStage stage);
        /// <summary>
        /// Execute the pipeline on a message
        /// </summary>
        /// <param name="message">Received message</param>
        /// <returns>Response message</returns>
        IMessage Execute(IMessage message);
    }
}
