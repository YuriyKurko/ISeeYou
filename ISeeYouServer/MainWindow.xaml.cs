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

namespace ISeeYouServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private TcpClient client;
        private TcpListener server;
        private NetworkStream mainStream;

        private Thread Listening;
        private Thread GetImage;

        private void StartListening()
        {
            while (!client.Connected)
            {
                server.Start();
                client = server.AcceptTcpClient();
            }
        }

        private void StopListening()
        {
            server.Stop();
            client = null;
            if (Listening.IsAlive) Listening.Abort();
            if (GetImage.IsAlive) GetImage.Abort();
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void ReceiveImage()
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            while (client.Connected)
            {
                mainStream = client.GetStream();
                Bitmap b = new Bitmap((System.Drawing.Image)binFormatter.Deserialize(mainStream));

                Action action = delegate { screenImage.Source = BitmapToImageSource(b); };
                screenImage.Dispatcher.Invoke(action);
            }
        }

        private void listenButton_Click(object sender, RoutedEventArgs e)
        {
            int port = int.Parse(portTextBox.Text);
            client = new TcpClient();
            server = new TcpListener(IPAddress.Any, port);
            Listening = new Thread(StartListening);
            Listening.Start();
        }

       
        private void TabItem_Selected(object sender, RoutedEventArgs e)
        {
            if(GetImage == null)
            {
                GetImage = new Thread(ReceiveImage);
                GetImage.Start();
            }
            else if(GetImage.IsAlive == false)
            {
                GetImage.Start();
            }
        }

        
    }
}
