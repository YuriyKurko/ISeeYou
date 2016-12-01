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
    /// Interaction logic for ShareScreenWindow.xaml
    /// </summary>
    public partial class ShareScreenWindow : Window
    {
        public ShareScreenWindow(ServerWindow serverWindow, DataGrid clientsDataGrid, ServerObject server)
        {
            InitializeComponent();
            this.serverWindow = serverWindow;
            this.clientsDataGrid = clientsDataGrid;
            this.server = server;
        }

        private Thread GetImage;
        private ServerWindow serverWindow { get; set; }
        private DataGrid clientsDataGrid { get; set; }
        ServerObject server { get; set; }


        private void shareScreenButton_Click(object sender, RoutedEventArgs e)
        {
            ReceiveScreenImage receivedImage = new ReceiveScreenImage(serverWindow.GetSelectedClient(), screenImage);

            if (shareScreenButton.Content.ToString() == "Start sharing")
            {
                server.SendCommand("START_SHARE_SCREEN", serverWindow.GetSelectedClient().Id);
                GetImage = new Thread(new ThreadStart(receivedImage.ReceiveImage));
                GetImage.Start();
                shareScreenButton.Content = "Stop sharing";
                clientsDataGrid.IsEnabled = false;
            }
            else
            {
                server.SendCommand("STOP_SHARE_SCREEN", serverWindow.GetSelectedClient().Id);
                shareScreenButton.Content = "Start sharing";
                GetImage.Abort();
                clientsDataGrid.IsEnabled = true;
            }
        }
    }
}
