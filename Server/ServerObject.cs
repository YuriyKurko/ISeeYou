using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Server
{
    public class ServerObject
    {
        public ServerObject(int port, DataGrid clientsDataGrid)
        {
            this.port = port;
            this.clientsDataGrid = clientsDataGrid;
        }

        static TcpListener tcpListener; // сервер для прослушивания
        public TcpClient tcpClient;
        DataGrid clientsDataGrid;
        public List<ClientObject> clients = new List<ClientObject>(); // все подключения

        public int port { get; set; }

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);

            if (client != null)
                clients.Remove(client);
        }
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();
                Console.WriteLine("Server started. Waiting connections...");

                while (true)
                {
                    tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);

                    clientsDataGrid.Dispatcher.Invoke(() => this.clientsDataGrid.Items.Clear());//new Action(() => this.clientsListView.Items.Clear()));

                    foreach (var c in clients)
                    {
                        Action action = delegate
                        {
                            clientsDataGrid.Items.Add(c);
                            clientsDataGrid.Items.Refresh();
                        };
                        clientsDataGrid.Dispatcher.Invoke(action);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        public void SendCommand(string command, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(command);
            foreach(var c in clients)
            {
                if (c.Id == id)
                {
                    c.Stream.Write(data, 0, data.Length);
                }
            }
        }

        protected internal void Disconnect()
        {
            if (tcpListener != null)
            {
                tcpListener.Stop(); 

                for (int i = 0; i < clients.Count; i++)
                {
                    clients[i].Close();
                }
            }

            Action action = delegate
            {
                clientsDataGrid.Items.Clear();
            };
            clientsDataGrid.Dispatcher.Invoke(action);
        }
    }
}
