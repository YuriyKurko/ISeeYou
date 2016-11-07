using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Net;
using System.Windows.Interop;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {
        public ServerWindow()
        {
            InitializeComponent();
        }

        static ServerObject server; 
        static Thread listenThread; 
        private Thread GetImage;
        private Thread Sendessage;

        private void listenButton_Click(object sender, RoutedEventArgs e)
        {
            int port = int.Parse(portTextBox.Text);

            try
            {
                server = new ServerObject(port, clientsListView);
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start(); 
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            server.Disconnect();
        }

        private void shareScreenButton_Click(object sender, RoutedEventArgs e)
        {
            ReceiveScreenImage receivedImage = new ReceiveScreenImage(server.clients[0], screenImage);

            if(shareScreenButton.Content.ToString() == "Start sharing")
            {
                server.SendCommand("START_SHARE_SCREEN", 0);
                GetImage = new Thread(new ThreadStart(receivedImage.ReceiveImage));
                GetImage.Start();
                shareScreenButton.Content = "Stop sharing";
            }
            else
            {
                server.SendCommand("STOP_SHARE_SCREEN", 0);
                shareScreenButton.Content = "Start sharing";
                GetImage.Abort();
            }
        }

        private void sendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if(messageTextBox.Text.ToString() != "")
            {
                server.SendCommand("MESSAGEBOX|" + messageTextBox.Text, 0);
            }
            else
            {
                MessageBox.Show("Enter message", "Empty message",MessageBoxButton.OK ,MessageBoxImage.Asterisk);
            }

        }
    }
}
