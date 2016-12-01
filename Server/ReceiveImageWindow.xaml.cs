using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Server
{
    /// <summary>
    /// Interaction logic for ShareWebcamWindow.xaml
    /// </summary>
    public partial class ReceiveImageWindow : Window
    {
        public ReceiveImageWindow(ServerWindow serverWindow, DataGrid clientsDataGrid, ServerObject server)
        {
            InitializeComponent();
            this.serverWindow = serverWindow;
            this.clientsDataGrid = clientsDataGrid;
            this.server = server;
        }

        private Thread GetScreenImage;
        private Thread GetWebcamImage;
        private ServerWindow serverWindow { get; set; }
        private DataGrid clientsDataGrid { get; set; }
        ServerObject server { get; set; }


        private void shareWebcamButton_Click(object sender, RoutedEventArgs e)
        {
            ReceiveImage receiveImage = new ReceiveImage(serverWindow.GetSelectedClient(), screenImage);

            if (shareWebcamButton.Content.ToString() == "Start webcam")
            {
                server.SendCommand("START_WEBCAM", serverWindow.GetSelectedClient().Id);
                GetWebcamImage = new Thread(new ThreadStart(receiveImage.ReceiveImageData));
                GetWebcamImage.Start();
                shareWebcamButton.Content = "Stop webcam";
                clientsDataGrid.IsEnabled = false;
            }
            else
            {
                server.SendCommand("STOP_WEBCAM", serverWindow.GetSelectedClient().Id);
                shareWebcamButton.Content = "Start webcam";
                GetWebcamImage.Abort();
                receiveImage = null;
                clientsDataGrid.IsEnabled = true;
            }
        }

        private void shareScreenButton_Click(object sender, RoutedEventArgs e)
        {
            ReceiveImage receiveImage = new ReceiveImage(serverWindow.GetSelectedClient(), screenImage);

            if (shareScreenButton.Content.ToString() == "Start sharing screen")
            {
                server.SendCommand("START_SHARE_SCREEN", serverWindow.GetSelectedClient().Id);
                GetScreenImage = new Thread(new ThreadStart(receiveImage.ReceiveImageData));
                GetScreenImage.Start();
                shareScreenButton.Content = "Stop sharing screen";
                clientsDataGrid.IsEnabled = false;
            }
            else
            {
                server.SendCommand("STOP_SHARE_SCREEN", serverWindow.GetSelectedClient().Id);
                shareScreenButton.Content = "Start sharing screen";
                GetScreenImage.Abort();
                receiveImage = null;
                clientsDataGrid.IsEnabled = true;
            }

        }
    }
}
