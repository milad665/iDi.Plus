using System;
using System.Net.Sockets;
using System.Threading;

namespace iDi.Plus.Test.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var tcpClient = new TcpClient();
            tcpClient.Connect("localhost", 28694);
            var stream = tcpClient.GetStream();
            Thread.Sleep(600);

            stream.Write(new byte[5000], 0, 5000);

            Console.Read();
        }
    }
}
