using System;
using System.Net;
using System.Net.Sockets;

namespace iDi.Plus.Application
{
    public class Process
    {
        private readonly Settings _settings;

        public Process(Settings settings)
        {
            _settings = settings;
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
            var listener = new UdpClient(port);
            var groupEp = new IPEndPoint(IPAddress.Any, port);
            Console.WriteLine("Waiting for broadcast");

            while (true)
            {
                try
                {
                    byte[] bytes = listener.Receive(ref groupEp);

                    Console.WriteLine($"{DateTime.UtcNow}: Packet Received from {groupEp}");
                    Console.WriteLine(BitConverter.ToString(bytes));

                    var packet = new LmDirectPacket(bytes);
                    Console.WriteLine(
                        $"{DateTime.UtcNow}: Packet Decoded.  MobileId:{Enum.GetName(typeof(MobileIdTypes), packet.OptionsHeader.MobileIdType)}: {packet.OptionsHeader.MobileId}");

                    if (packet.MessageHeader.MessageType == MessageHeader.MessageTypes.EventReport)
                    {
                        var result = SendSensorData(packet);
                        Console.WriteLine($"Packet Sent to Core. Result: {result}");
                    }

                    //Console.WriteLine(packet.ToString());

                    if (packet.Content.GetType() == typeof(EventReportMessage))
                    {
                        var temperature = ((EventReportMessage)packet.Content).Accumulators[1]?.GetAnalogValue(0.0625);
                        var humidity = ((EventReportMessage)packet.Content).Accumulators[2]?.GetAnalogValue(0.0625);

                        Console.WriteLine($"\tTemperature C: {temperature}");
                        Console.WriteLine($"\tTemperature F: {(temperature * 9 / 5) + 32}");
                        Console.WriteLine($"\tHumidity: {humidity}");
                    }


                    var ack = packet.GetAcknowledge(true);

                    if (ack != null)
                    {
                        listener.Send(ack.RawData, ack.RawData.Length, groupEp);
                        Console.WriteLine($"{DateTime.UtcNow}: Ack Sent to {groupEp}");
                    }

                    SendForceCheckinRequest(listener, groupEp, packet);

                    Console.WriteLine(string.Join('*', new string[70]));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}