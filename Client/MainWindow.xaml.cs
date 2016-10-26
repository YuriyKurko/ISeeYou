using SlimDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            ShowInTaskbar = false;
            InitializeComponent();
        }

        
        private readonly TcpClient client = new TcpClient();
        private NetworkStream mainStream;
        private int portNumber;

        //private static System.Drawing.Image GrabDesktop()
        //{
        //    DxScreenCapture sc = new DxScreenCapture();
        //    Surface s = sc.CaptureScreen();
        //    Bitmap screenshot = new Bitmap(Surface.ToStream(s, ImageFileFormat.Bmp));

        //    return screenshot;
        //}

        private static System.Drawing.Image GrabDesktop()
        {
            System.Drawing.Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screeshot = new Bitmap(bounds.Width, bounds.Height);
            Graphics graphic = Graphics.FromImage(screeshot);
            graphic.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
            return screeshot;
        }

        private void SendScreenshot()
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            mainStream = client.GetStream();
            binFormatter.Serialize(mainStream, GrabDesktop());
        }

        private void Connect()
        {
            try
            {
                portNumber = int.Parse(portTextBox.Text);
                client.Connect(ipHostTextBox.Text, portNumber);
                System.Windows.MessageBox.Show("Connected");
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        private void shareWindowButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            if (shareWindowButton.Content.ToString().StartsWith("Share"))
            {
                dispatcherTimer.Start();
                shareWindowButton.Content = "Stop sharing";
            }
            else
            {
                dispatcherTimer.Stop();
                shareWindowButton.Content = "Share my screen";
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            SendScreenshot();
        }
    }
}
