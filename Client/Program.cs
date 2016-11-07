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
            while (true)
            {
                try
                {
                    tcpClient = new TcpClient();
                    client = new Client(Environment.MachineName, CultureInfo.CurrentCulture.DisplayName, tcpClient);

                    client.Connect("192.168.0.59", 1488);

                    Thread getCommandThread = new Thread(new ThreadStart(client.GetCommand));
                    getCommandThread.Start();

                    Console.ReadKey();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    client.Disconnect();
                }
            }
        }
    }
}