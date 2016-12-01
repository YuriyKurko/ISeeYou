using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Server
{
    public class ClientObject
    {
        public string Id { get; private set; }
        public string IP { get; private set; }
        public string MachineName { get; private set; }
        public string UserName { get; private set; }
        public string Country { get; private set; }

        public bool isCheked = true;

        protected internal NetworkStream Stream { get; set; }
        public TcpClient tcpClient;
        ServerObject server; 

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();

            this.tcpClient = tcpClient;
            server = serverObject;
            Stream = tcpClient.GetStream();

            IP = tcpClient.Client.RemoteEndPoint.ToString();

            GetClientInfo();

            serverObject.AddConnection(this);
        }

        private void GetClientInfo()
        {
            int bytes = 0;
            string[] receivedMessage;
            byte[] data = new byte[256];

            StringBuilder builder = new StringBuilder();

            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            receivedMessage = builder.ToString().Split('|');

            MachineName = receivedMessage[0];
            UserName = receivedMessage[1];
            Country = receivedMessage[2];
        }

        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (tcpClient != null)
                tcpClient.Close();
        }
    }
}
