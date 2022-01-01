using iDi.Blockchain.Framework.Execution;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Blockchain.Framework.Server
{
    public class DefaultBlockchainNodeServer : IBlockchainNodeServer
    {
        private readonly PipelineFactory _pipelineFactory;

        public DefaultBlockchainNodeServer(PipelineFactory pipelineFactory)
        {
            _pipelineFactory = pipelineFactory;
        }

        public void Listen(int port)
        {
            var serverEndpoint = new IPEndPoint(IPAddress.Any, port);
            var listener = new TcpListener(serverEndpoint);
            listener.Start();

            while (true)
            {
                var client = listener.AcceptTcpClient();
                Task.Run(() => HandleConnection(client));
            }
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

                var message = Message.FromMessageData(messageStream.ToArray());
                var request = new RequestContext(message, client.Client.LocalEndPoint, client.Client.RemoteEndPoint);
                var pipeline = _pipelineFactory.Create();
                var response = pipeline.Execute(request);
                networkStream.Write(response.RawData);
                networkStream.Close(); // Send data
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
            }
            finally
            {
                client.Close();
            }
        }
    }
}
