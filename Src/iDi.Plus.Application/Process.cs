using iDi.Blockchain.Core;
using iDi.Blockchain.Core.Execution;
using iDi.Protocol.iDiDirect;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace iDi.Plus.Application
{
    public class Process
    {
        private readonly Settings _settings;
        private readonly IPipeline _pipeline;

        public Process(Settings settings, IPipeline pipeline)
        {
            _settings = settings;
            _pipeline = pipeline;
        }

        public void Run()
        {
            // Load DNSs
            // Update node addresses (Max 1000 random)
            // Download/Update blockchain
            
            Listen(_settings.Port);
        }

        public void Listen(int port)
        {
            var serverEndpoint = new IPEndPoint(IPAddress.Any, port);
            var listener = new TcpListener(serverEndpoint);
            listener.Start();

            while (true)
            {
                Console.WriteLine($"{DateTime.Now}: Waiting for connection...");
                var client = listener.AcceptTcpClient();
                Console.WriteLine($"{DateTime.Now}: {client.Client.RemoteEndPoint} Connected.");
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
                Console.WriteLine($"{DateTime.Now}: [{messageStream.Length}] bytes received.");
                var message = Message.FromMessageData(messageStream.ToArray());
                var request = new RequestContext(message, client.Client.LocalEndPoint, client.Client.RemoteEndPoint);
                var response = _pipeline.Execute(request);
                networkStream.Write(response.RawData);
                networkStream.Close(); // Send data
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                client.Close();
            }
        }
    }
}