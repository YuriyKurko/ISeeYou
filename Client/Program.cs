using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Client
{
    class Program
    {
        static TcpClient tcpClient;
        static Client client;
        static void Main(string[] args)
        {
            string host = "192.168.0.59";
            int port = 1488;

            while(true)
            {
                try
                {
                    tcpClient = new TcpClient();
                    client = new Client(Environment.MachineName, System.Security.Principal.WindowsIdentity.GetCurrent().Name ,CultureInfo.CurrentCulture.DisplayName, tcpClient);
                    client.Connect(host, port);
                    client.Process();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    client.Disconnect();
                }
            }
        }
    }
}