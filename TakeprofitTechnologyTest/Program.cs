using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TakeprofitTechnologyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string server = "88.212.241.115";
            int port = 2013;
            string receivedMessage;

            for (int i = 1; i < 10; i++)
            {
                TcpClient client = Connect(server, port);
                Stream stream = OpenStream(client);

                ReadServerMessage(stream);

                // SendServerMessage("Greetings\n", stream);

                SendServerMessage($"{i}\n", stream);

                Thread.Sleep(1);
                Console.WriteLine("End of read");


                receivedMessage = ReadServerMessage(stream);
                Console.WriteLine(receivedMessage);

                //File.WriteAllText("taskNew.txt", receivedMessage);


                Disconnect(client, stream);
            }
            Console.ReadKey();
        }

        static TcpClient Connect(String server, Int32 port)
        {
            try
            {
                TcpClient client = new TcpClient(server, port);
                Console.WriteLine($"Connection status = {client.Connected}");
                return client;

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
                throw new Exception("ArgumentNullException");
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                throw new Exception("ArgumentNullException");
            }
        }

        static void Disconnect(TcpClient client, Stream stream)
        {
            CloseStream(stream);
            if (client != null)
            {
                client.Close();
            }
        }

        static Stream OpenStream(TcpClient client)
        {
            return client.GetStream();
        }

        static void CloseStream(Stream stream)
        {
            if (stream != null)
            {
                stream.Close();
            }
        }

        static string ReadServerMessage(Stream stream)
        {
            Byte[] data = new Byte[2048];
            String responseData = String.Empty;
            Int32 bytes = stream.Read(data, 0, data.Length);
            Console.WriteLine($"Read {bytes} bytes");
            responseData = System.Text.Encoding.GetEncoding("KOI8-R").GetString(data, 0, bytes);

            Console.WriteLine("Received: {0}", responseData);
            return responseData;
        }
        static async Task<string> ReadServerMessageAsync(Stream stream)
        {
            Byte[] data = new Byte[2048];

            String responseData = String.Empty;
            //Int32 bytes = stream.Read(data, 0, data.Length);
            using (stream)
            {
                await stream.ReadAsync(data, 0, data.Length).ConfigureAwait(false);
            }

            var x = stream.CanRead;
            Console.WriteLine($"Read Async {data.Length} bytes");
            responseData = System.Text.Encoding.GetEncoding("KOI8-R").GetString(data, 0, data.Length);

            Console.WriteLine($"End Read Async length = {responseData.Length}");
            return responseData;
        }

        static void SendServerMessage(string message, Stream stream)
        {
            Byte[] data = System.Text.Encoding.GetEncoding("KOI8-R").GetBytes(message);
            stream.Write(data, 0, data.Length);
            Console.WriteLine("Sent: {0}", message);
        }
    }

}
