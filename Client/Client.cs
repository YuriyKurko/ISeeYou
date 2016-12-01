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
        public Client(string MachineName, string UserName, string County, TcpClient Client)
        {
            machineName = MachineName;
            userName = UserName;
            country = County;
            tcpClient = Client;

        }

        public bool isConnected = false;
        bool isScreenStarted = false;
        bool isWebcamStarted = false;

        private TcpClient tcpClient;
        public NetworkStream networkStream;

        private ShareScreen shareScreen;
        private ShareWebcam shareWebcam;
        private Thread shareScreenThread;
        private Thread shareWebcamThread;

        private string machineName { get; set; }
        private string userName { get; set; }
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
                    SendClientInfo();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void Process()
        {
            while (tcpClient.Connected)
            {
                try
                {
                    int bytes = 0;
                    string message = "";
                    string command = "";
                    string[] receivedMessage;
                    byte[] data = new byte[256];

                    StringBuilder builder = new StringBuilder();

                    networkStream = tcpClient.GetStream();

                    do
                    {
                        bytes = networkStream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (networkStream.DataAvailable);

                    receivedMessage = builder.ToString().Split('|');
                    command = receivedMessage[0];

                    if (receivedMessage.Count() == 2)
                    {
                        message = receivedMessage[1];
                    }
                    switch (command)
                    {
                        case "START_SHARE_SCREEN":
                            if (!isWebcamStarted)
                            {
                                shareScreen = new ShareScreen(tcpClient.GetStream());
                                shareScreenThread = new Thread(new ThreadStart(shareScreen.SendScreenshot));
                                shareScreenThread.Start();
                                isScreenStarted = true;
                            }
                            else
                            {
                                shareWebcam.StopCapturing();
                                shareWebcamThread.Abort();
                                shareWebcamThread = null;
                                shareWebcam = null;
                                isWebcamStarted = false;

                                shareWebcam = new ShareWebcam(tcpClient.GetStream(), true);
                                shareWebcamThread = new Thread(new ThreadStart(shareWebcam.CapturePhoto));
                                shareWebcamThread.Start();
                                isWebcamStarted = true;
                            }
                            break;
                        case "STOP_SHARE_SCREEN":
                            shareScreenThread.Abort();
                            shareScreenThread = null;
                            shareScreen = null;
                            isScreenStarted = false;
                            break;
                        case "START_WEBCAM":
                            //if (!isScreenStarted)
                            //{
                            //    shareWebcam = new ShareWebcam(tcpClient.GetStream(), false);
                            //    shareWebcamThread = new Thread(new ThreadStart(shareWebcam.CapturePhoto));
                            //    shareWebcamThread.Start();
                            //    isWebcamStarted = true;
                            //}
                            //else
                            //{
                                shareScreenThread.Abort();
                                shareScreenThread = null;
                                shareScreen = null;
                                isScreenStarted = false;

                                shareWebcam = new ShareWebcam(tcpClient.GetStream(), true);
                                shareWebcamThread = new Thread(new ThreadStart(shareWebcam.CapturePhoto));
                                shareWebcamThread.Start();
                                isWebcamStarted = true;
                            //}
                            break;
                        case "STOP_WEBCAM":
                            shareWebcam.StopCapturing();
                            shareWebcamThread.Abort();
                            shareWebcamThread = null;
                            shareWebcam = null;
                            isWebcamStarted = false;
                            break;
                        case "MESSAGEBOX":
                            MessageBox.Show(message, "Windows message", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            break;
                    }

                    if (command != "")
                    {
                        Console.WriteLine(command);
                    }
                    else
                    {
                        throw new Exception();
                    }

                }
                catch(Exception ex)
                {
                    Console.WriteLine("Connection interrupted! " + ex.Message);
                    Disconnect();
                }
            }
        }

        public void SendClientInfo()
        {
            string message = $"{machineName}|{userName}|{country}";
            byte[] data = Encoding.Unicode.GetBytes(message);
            networkStream = tcpClient.GetStream();
            networkStream.Write(data, 0, data.Length);
        }

        public void Disconnect()
        {
            if (networkStream != null)
                networkStream.Close();
            if (tcpClient != null)
                tcpClient.Close();
        }

        
    }
}
