using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    class Client
    {
        public Client(string MachineName, string County, TcpClient Client)
        {
            machineName = MachineName;
            country = County;
            tcpClient = Client;

        }

        public bool isConnected = false;

        private readonly TcpClient tcpClient = new TcpClient();
        public static NetworkStream networkStream;

        private ShareScreen shareScreen;
        private Thread shareScreenThread;

        private string machineName { get; set; }
        private string country { get; set; }

        public void Connect(string host, int port)
        {
            while (!isConnected)
            {
                try
                {
                    tcpClient.Connect(host, port);
                    Console.WriteLine($"User {machineName} connected to {tcpClient.Client.RemoteEndPoint}.");
                    isConnected = tcpClient.Connected;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void GetMessage()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = networkStream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (networkStream.DataAvailable);

            MessageBox.Show(builder.ToString(), "Windows message", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        public void GetCommand()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[256];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    networkStream = tcpClient.GetStream();
                    do
                    {
                        bytes = networkStream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (networkStream.DataAvailable);

                    string message = "";

                    string[] receivedMessage = builder.ToString().Split('|');

                    string command = receivedMessage[0];

                    if (receivedMessage.Count() == 2)
                    {
                        message = receivedMessage[1];
                    }

                    Console.WriteLine(command);

                    if(command == "START_SHARE_SCREEN")
                    {
                        shareScreen = new ShareScreen(tcpClient.GetStream());
                        shareScreenThread = new Thread(new ThreadStart(shareScreen.SendScreenshot));
                        shareScreenThread.Start();  
                    }
                    else if(command == "STOP_SHARE_SCREEN")
                    {
                        shareScreenThread.Abort();
                        shareScreenThread = null;
                        shareScreen = null;
                    }
                    else if(command == "MESSAGEBOX")
                    {
                        MessageBox.Show(message, "Windows message", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }
                catch
                {
                    Console.WriteLine("Connection interrupted!"); 
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        public void Disconnect()
        {
            if (networkStream != null)
                networkStream.Close();//отключение потока
            if (tcpClient != null)
                tcpClient.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }

        
    }
}
