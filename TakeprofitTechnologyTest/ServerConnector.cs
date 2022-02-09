using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace TakeprofitTechnologyTest
{
    public class ServerConnector
    {
        private string ipAdress;
        private int port;
        protected TcpClient server;

        public ServerConnector(string serverAdress, int serverPort)
        {
            ipAdress = serverAdress;
            port = serverPort;
            ConnectServer();
        }

        public void ConnectServer()
        {
            CloseConnection();
            server = new TcpClient(ipAdress, port);
           // Console.WriteLine($"Connection status = {server.Connected}");
        }

        public virtual void CloseConnection()
        {
            if (server != null)
            {
                server.Close();
            }
        }
    }
}
