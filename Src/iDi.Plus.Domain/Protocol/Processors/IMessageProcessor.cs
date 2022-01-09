using iDi.Blockchain.Framework.Protocol;

namespace iDi.Plus.Domain.Protocol.Processors;

public interface IMessageProcessor
{
    MessageTypes MessageType { get; }
    /// <summary>
    /// Processes the incoming message
    /// </summary>
    /// <param name="message"></param>
    /// <returns>If a response should be sent back to the sender node, returns the response message, otherwise null</returns>
    Message Process(Message message);
    bool CanProcess(Message message);
}