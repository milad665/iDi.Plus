using System;

namespace iDi.Blockchain.Framework.Protocol;

public interface IMessageFactory
{
    /// <summary>
    /// Creates a new message instance from received bytes.
    /// </summary>
    /// <param name="messageData"></param>
    /// <returns></returns>
    Message CreateMessage(ReadOnlySpan<byte> messageData);
}