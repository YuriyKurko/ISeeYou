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
        public ServerObject(int port, ListView clientsListView)
        {
            this.port = port;
            this.clientsListView = clientsListView;
        }

        static TcpListener tcpListener; // сервер для прослушивания
        public TcpClient tcpClient;
        ListView clientsListView;
        public List<ClientObject> clients = new List<ClientObject>(); // все подключения

        public int port { get; set; }

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null)
                clients.Remove(client);
        }
        // прослушивание входящих подключений
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

                    clientsListView.Dispatcher.Invoke(() => this.clientsListView.Items.Clear());//new Action(() => this.clientsListView.Items.Clear()));

                    foreach (var c in clients)
                    {
                        Action action = delegate
                        {
                            clientsListView.Items.Add(c.tcpClient.Client.RemoteEndPoint);
                            clientsListView.Items.Refresh();
                        };
                        clientsListView.Dispatcher.Invoke(action);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        public void SendCommand(string command, int id)
        {
            byte[] data = Encoding.Unicode.GetBytes(command);
            clients[id].Stream.Write(data, 0, data.Length); //передача данных
        }

        // отключение всех клиентов
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }
    }
}
