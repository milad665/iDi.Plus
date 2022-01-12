using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using iDi.Blockchain.Framework.Execution;
using iDi.Blockchain.Framework.Protocol;
using iDi.Blockchain.Framework.Providers;

namespace iDi.Blockchain.Framework.Communication
{
    public class DefaultBlockchainNodeServer : IBlockchainNodeServer
    {
        private readonly IPipelineFactory _pipelineFactory;
        private readonly ILocalNodeContextProvider _localNodeContextProvider;
        private readonly IMessageFactory _messageFactory;

        public DefaultBlockchainNodeServer(IPipelineFactory pipelineFactory, ILocalNodeContextProvider localNodeContextProvider, IMessageFactory messageFactory)
        {
            _pipelineFactory = pipelineFactory;
            _localNodeContextProvider = localNodeContextProvider;
            _messageFactory = messageFactory;
        }

        public void Listen(int port, CancellationToken cancellationToken)
        {
            var serverEndpoint = new IPEndPoint(IPAddress.Any, port);
            var listener = new TcpListener(serverEndpoint);
            listener.Start();

            while (!cancellationToken.IsCancellationRequested)
            {
                var client = listener.AcceptTcpClient();
                Task.Run(() => HandleConnection(client, cancellationToken), cancellationToken);
            }
            listener.Stop();
        }

        private void HandleConnection(TcpClient client, CancellationToken cancellationToken)
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
                while (networkStream.DataAvailable && !cancellationToken.IsCancellationRequested)
                {
                    readBytesCount = networkStream.Read(buffer, 0, buffer.Length);
                    messageStream.Write(buffer, 0, readBytesCount);
                }
                
                var message = _messageFactory.CreateMessage(messageStream.ToArray());
                var request = new RequestContext(message, client.Client.LocalEndPoint, client.Client.RemoteEndPoint, _localNodeContextProvider.LocalKeys);
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
