using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using CITYMumbler.Common.Contracts.Services.Logger;
using CITYMumbler.Networking.Sockets;

namespace CITYMumbler.Server
{
    public class MumblerServer
    {
        private TcpSocketListener listener;
        private ObservableCollection<Client> connectedClients;
        private int idSeed = 1;
        private ILogger logger;
        public int Port { get; private set; } = 21992;
        public BehaviorSubject<bool> IsRunning { get; private set; }

        public MumblerServer(ILoggerService loggerService)
        {
            this.connectedClients = new ObservableCollection<Client>();
            this.logger = loggerService.GetLogger(this.GetType());
            this.listener = new TcpSocketListener();
            this.IsRunning = new BehaviorSubject<bool>(false);
        }

        public void Start(int port = 21992)
        {
            this.Port = port;
            this.listener.OnAccepted += OnAccepted_Callback;
            this.listener.Start(port);
            this.IsRunning.OnNext(true);
            this.logger.Log(LogLevel.Info, "Server listening on port: {0}...", this.Port);
        }

        public void Stop()
        {
            foreach (var client in connectedClients)
            {
                client.ClientSocket.Disconnect();
            }
            this.listener.OnAccepted -= OnAccepted_Callback;
            this.IsRunning.OnNext(false);
            this.listener.Stop();
            
        }

        private void OnAccepted_Callback(object sender, OnAcceptedTcpSocketEventArgs args)
        {
            this.logger.Log(LogLevel.Info, "Accepted new connection from: {0}", args.RemoteEndpoint);
            Client newClient = new Client
            {
                ID = idSeed++,
                ClientSocket = new TcpSocket(args.AcceptedSocket),
                RemoteEndpoint = args.RemoteEndpoint
            };
            this.connectedClients.Add(newClient);

            newClient.ClientSocket.OnDataReceived += ClientSocket_OnDataReceived;
            newClient.ClientSocket.OnDisconnected += (ds, de) =>
            {
                connectedClients.Remove(newClient);
                this.logger.Log(LogLevel.Warn, "Connection lost with {0}", newClient.RemoteEndpoint);
            };

        }


        private void ClientSocket_OnDataReceived(object sender, TcpSocketDataReceivedEventArgs e)
        {
            string message = System.Text.Encoding.ASCII.GetString(e.Payload);

            this.logger.Log(LogLevel.Debug, "Received : {0}", message);
            //Console.WriteLine("received");
            foreach (var c in connectedClients)
            {
                c.ClientSocket.Send(e.Payload);
            }
        }
   }
}
