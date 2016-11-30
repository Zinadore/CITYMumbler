using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CITYMumbler.Networking.Sockets;

namespace CITYMumbler.Server
{
    public class MumblerServer
    {
        private TcpSocketListener listener;
        private ObservableCollection<Client> connectedClients;
        private int idSeed = 1;

        public MumblerServer(int port = 21992)
        {
            this.listener = new TcpSocketListener(port);
            this.connectedClients = new ObservableCollection<Client>();

        }

        public void Start()
        {
            this.listener.OnAccepted += OnAccepted_Callback;
            this.listener.Start();
        }

        public void Stop()
        {
            this.listener.OnAccepted -= OnAccepted_Callback;
            this.listener.Stop();
        }

        private void OnAccepted_Callback(object sender, OnAcceptedTcpSocketEventArgs args)
        {
            Log("Accepted new connection from: {0}", args.RemoteEndpoint);
            Client newClient = new Client
            {
                ID = idSeed++,
                ClientSocket = new TcpSocket(args.AcceptedSocket),
                RemoteEndpoint = args.RemoteEndpoint
            };
            this.connectedClients.Add(newClient);

            newClient.ClientSocket.OnDataReceived += ClientSocket_OnDataReceived;
        }

        private void ClientSocket_OnDataReceived(object sender, TcpSocketDataReceivedEventArgs e)
        {
            //Console.WriteLine("received");
            string message = System.Text.Encoding.UTF32.GetString(e.Payload);
            Log("Received: " + message);
            foreach (var c in connectedClients)
            {
                c.ClientSocket.Send(e.Payload);
            }
        }

        private void Log(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

    }
}
