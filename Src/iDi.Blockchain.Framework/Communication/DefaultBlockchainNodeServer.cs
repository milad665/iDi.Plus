using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using iDi.Blockchain.Framework.Cryptography;
using iDi.Blockchain.Framework.Execution;
using iDi.Blockchain.Framework.Protocol;

namespace iDi.Blockchain.Framework.Communication
{
    public class DefaultBlockchainNodeServer : IBlockchainNodeServer
    {
        private readonly PipelineFactory _pipelineFactory;

        public DefaultBlockchainNodeServer(PipelineFactory pipelineFactory)
        {
            _pipelineFactory = pipelineFactory;
        }

        public void Listen(int port, CancellationToken cancellationToken, ReadOnlyDictionary<string, BlockchainNode> nodesList, KeyPair localKeys)
        {
            var serverEndpoint = new IPEndPoint(IPAddress.Any, port);
            var listener = new TcpListener(serverEndpoint);
            listener.Start();

            while (!cancellationToken.IsCancellationRequested)
            {
                var client = listener.AcceptTcpClient();
                Task.Run(() => HandleConnection(client, cancellationToken, nodesList, localKeys), cancellationToken);
            }
        }

        private void HandleConnection(TcpClient client, CancellationToken cancellationToken, ReadOnlyDictionary<string, BlockchainNode> nodesList, KeyPair localKeys)
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
                
                var message = Message.FromMessageData(messageStream.ToArray());
                var request = new RequestContext(message, client.Client.LocalEndPoint, client.Client.RemoteEndPoint, localKeys);
                var pipeline = _pipelineFactory.Create(nodesList);
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
