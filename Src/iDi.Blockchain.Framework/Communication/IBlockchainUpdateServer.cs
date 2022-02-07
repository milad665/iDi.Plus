
using System;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Blockchain.Framework.Communication;

public interface IBlockchainUpdateServer
{
    public event Action<IBlockchainUpdateServer, MessageReceivedEventArgs> WitnessNodesListMessageReceived;
    public event Action<IBlockchainUpdateServer, MessageReceivedEventArgs> NewBlocksMessageReceived;
    public event Action<IBlockchainUpdateServer, MessageReceivedEventArgs> BlockDataMessageReceived;
    public event Action ServerStarted;
    public event Action AllBlocksReceived;

    void Listen(int port);
}

public class MessageReceivedEventArgs : EventArgs
{
    public MessageReceivedEventArgs(Message message)
    {
        Message = message;
        ReceiveTime = DateTime.UtcNow;
    }
    public Message Message { get; }
    public DateTime ReceiveTime { get; }
}