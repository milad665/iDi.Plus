using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Blockchain.Framework.Communication;

public class DefaultBlockchainNodeClient : IBlockchainNodeClient
{
    /// <summary>
    /// Sends a message to a node and received the response
    /// </summary>
    /// <param name="remoteEndpoint">Remote node endpoint</param>
    /// <param name="messageToSend"></param>
    /// <returns>Response message</returns>
    public Message Send(IPEndPoint remoteEndpoint, Message messageToSend)
    {
        var tcpClient = new TcpClient();
        tcpClient.Connect(remoteEndpoint.Address, remoteEndpoint.Port);
        var networkStream = tcpClient.GetStream();
        networkStream.Write(messageToSend.RawData, 0, messageToSend.RawData.Length);


        var buffer = new byte[tcpClient.ReceiveBufferSize];
        using var messageStream = new MemoryStream();

        // Wait to receive the first batch of response data
        var readBytesCount = networkStream.Read(buffer, 0, buffer.Length);
        messageStream.Write(buffer, 0, readBytesCount);

        //continue to read if there is more data
        while (networkStream.DataAvailable)
        {
            readBytesCount = networkStream.Read(buffer, 0, buffer.Length);
            messageStream.Write(buffer, 0, readBytesCount);
        }

        tcpClient.Close();
        return Message.FromMessageData(messageStream.ToArray());
    }
}