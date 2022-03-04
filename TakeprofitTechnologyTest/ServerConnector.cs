using System.Net.Sockets;

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
