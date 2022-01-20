using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using iDi.Blockchain.Framework.Communication;
using iDi.Blockchain.Framework.Protocol;
using iDi.Plus.Domain.Protocol.Payloads.MainNetwork.V1;

namespace iDi.Plus.Domain.Communication;

public class BlockchainUpdateServer : IBlockchainUpdateServer
{
    private readonly IMessageFactory _messageFactory;

    public event Action<IBlockchainUpdateServer, MessageReceivedEventArgs> NewBlocksMessageReceived;
    public event Action<IBlockchainUpdateServer, MessageReceivedEventArgs> BlockDataMessageReceived;
    public event Action ServerStarted;
    public event Action AllBlocksReceived;

    public BlockchainUpdateServer(IMessageFactory messageFactory)
    {
        _messageFactory = messageFactory;
    }

    public int NumberOfReceivedBlocks { get; private set; }
    public int TotalBlocks { get; private set; }

    public void Listen(int port)
    {
        var serverEndpoint = new IPEndPoint(IPAddress.Any, port);
        var listener = new TcpListener(serverEndpoint);
        listener.Start();

        ServerStarted?.Invoke();
        while (TotalBlocks == 0 || NumberOfReceivedBlocks < TotalBlocks)
        {
            var client = listener.AcceptTcpClient();
            HandleConnection(client);
        }
        listener.Stop();
        AllBlocksReceived?.Invoke();
    }

    private void HandleConnection(TcpClient client)
    {
        try
        {
            var buffer = new byte[client.ReceiveBufferSize];
            var networkStream = client.GetStream();
            networkStream.ReadTimeout = 500;
            using var messageStream = new MemoryStream();

            // Wait to receive some data
            var readBytesCount = networkStream.Read(buffer, 0, buffer.Length);
            messageStream.Write(buffer, 0, readBytesCount);

            //continue to read if there is more data
            while (networkStream.DataAvailable)
            {
                readBytesCount = networkStream.Read(buffer, 0, buffer.Length);
                messageStream.Write(buffer, 0, readBytesCount);
            }

            var message = _messageFactory.CreateMessage(messageStream.ToArray());
            if (message.Header.MessageType == MessageTypes.NewBlocks)
                ProcessNewBlocksMessage(message);
            else if (message.Header.MessageType == MessageTypes.BlockData)
                ProcessBlockDataMessage(message);

            networkStream.Close(); // Send data
        }
        catch (Exception)
        {
            //Console.WriteLine(e);
        }
        finally
        {
            client.Close();
        }
    }

    private void ProcessNewBlocksMessage(Message message)
    {
        TotalBlocks = ((NewBlocksPayload) message.Payload).Blocks.Count;
        NewBlocksMessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
    }

    private void ProcessBlockDataMessage(Message message)
    {
        NumberOfReceivedBlocks++;
        BlockDataMessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
    }
}