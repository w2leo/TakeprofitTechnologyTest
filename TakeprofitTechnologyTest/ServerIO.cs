using System.Net.Sockets;
using System.IO;
using System;
using System.Text;
using System.Threading;

namespace TakeprofitTechnologyTest
{
    public class ServerIO : ServerConnector
    {
        private const int MAX_BUFFER_SIZE = 64;
        private const int MAX_READ_TRY = 100;
        private const int MAX_TIMEOUT = 10000;
        private const int ERROR_CODE = -1;
        private Stream serverStream;

        public ServerIO(string serverAdress, int serverPort) : base(serverAdress, serverPort)
        {
            OpenStream();
        }

        private void OpenStream()
        {
            if (server != null)
            {
                serverStream = server.GetStream();
                SetTimeouts();
            }
        }

        private void SetTimeouts()
        {
            serverStream.ReadTimeout = MAX_TIMEOUT;
            serverStream.WriteTimeout = MAX_TIMEOUT;
        }

        private void CloseStream()
        {
            if (serverStream != null)
            {
                serverStream.Close();
            }
        }

        public override void CloseConnection()
        {
            CloseStream();
            base.CloseConnection();
        }

        private string ReadMessage()
        {
            string receivedString = string.Empty;
            int receivedBytes = 0;

            //do
            //{
            Byte[] data = new byte[MAX_BUFFER_SIZE];
            if (server.Connected)
            {
                receivedBytes = serverStream.Read(data, 0, data.Length);
                receivedString += (Encoding.GetEncoding("KOI8-R").GetString(data, 0, data.Length));
                //Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} : readPart : {receivedString}");
            }

            //   } while (receivedBytes == MAX_BUFFER_SIZE);
            return receivedString.ToString();
        }

        private void SendMessage(string message)
        {
            Byte[] data = Encoding.GetEncoding("KOI8-R").GetBytes(message);
            serverStream.Write(data, 0, data.Length);
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} : Sent: {message}");
        }

        public bool GetStringData(string inputData, out string receivedMessage)
        {
            ReadMessage();
            Thread.Sleep(10);
            receivedMessage = string.Empty;
            SendMessage(inputData);
            for (int i = 0; i < MAX_READ_TRY && server.Connected; i++)
            {
                receivedMessage += ReadMessage();
                if (receivedMessage.Contains("\n"))
                {
                    Console.WriteLine($"\nReceive completed : {ConvertDataToInt(receivedMessage)}");
                    return true;
                }
            }
            return false;
        }

        public int GetIntData(string inputData)
        {
            int result = ERROR_CODE;
            if (GetStringData(inputData, out string data)) ;
            {
                result = ConvertDataToInt(data);
            }
            return result;
        }

        private int ConvertDataToInt(string data)
        {
            if (!data.Contains("\n"))
            {
                return -1;
            }

            string resultString = RemoveTrashSymbols(data);

            if (int.TryParse(resultString, out int resultInt))
            {
                return resultInt;
            }
            else return -1;
        }

        private string RemoveTrashSymbols(string data)
        {
            string resultString = "";
            foreach (var c in data)
            {
                if (Char.IsDigit(c))
                    resultString += c;
                if (c == '\n')
                    break;
            }
            return resultString;
        }
    }

}




