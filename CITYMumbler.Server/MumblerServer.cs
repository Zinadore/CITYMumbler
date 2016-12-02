using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using CITYMumbler.Common.Contracts.Services.Logger;
using CITYMumbler.Networking.Contracts;
using CITYMumbler.Networking.Contracts.Serialization;
using CITYMumbler.Networking.Serialization;
using CITYMumbler.Networking.Sockets;

namespace CITYMumbler.Server
{
    /// <summary>
    /// The server part of the chat application. Encapsulates all the business logic related to handling clients
    /// and connections
    /// </summary>
    public class MumblerServer
    {
        #region Private Members
        // This is us
        private TcpSocketListener listener;
        private List<Client> connectedClients;
        // The idiot's way of doing IDs
        private ushort idSeed = 1;
        // Log all the things
        private ILogger logger;
        private IPacketSerializer _serializer;
        #endregion


        public int Port { get; private set; } = 21992;

        public BehaviorSubject<bool> IsRunning { get; private set; }

        public MumblerServer(ILoggerService loggerService)
        {
            this.connectedClients = new List<Client>();
            this.logger = loggerService.GetLogger(this.GetType());
            this.listener = new TcpSocketListener();
            this._serializer = new PacketSerializer();
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
            this.logger.Log(LogLevel.Warn, "Server terminated.");
        }

        private void OnAccepted_Callback(object sender, OnAcceptedTcpSocketEventArgs args)
        {
            this.logger.Log(LogLevel.Info, "Accepted new connection from: {0}", args.RemoteEndpoint);
            Client newClient = new Client(this.idSeed++, new TcpSocket(args.AcceptedSocket), args.RemoteEndpoint);
            this.connectedClients.Add(newClient);
            newClient.ClientSocket.OnDataReceived += ClientSocket_OnDataReceived;
            newClient.ClientSocket.OnDisconnected += ClientSocket_OnDisconnected;

        }


        private void ClientSocket_OnDataReceived(object sender, TcpSocketDataReceivedEventArgs e)
        {
            var receivedPacket = this._serializer.FromBytes(e.Payload);
            var socket = sender as TcpSocket;

            handlePacket(receivedPacket, socket);
        }

        private void ClientSocket_OnDisconnected(object sender, TcpSocketDisconnectedEventArgs e)
        {
            var disconnected = connectedClients.FirstOrDefault(client => client.ID == e.ClientID);
            connectedClients.Remove(disconnected);
            this.logger.Log(LogLevel.Warn, "Connection lost with {0}", disconnected?.RemoteEndpoint);
        }

        private void handlePacket(IPacket packet, TcpSocket clientSocket)
        {
            switch (packet.PacketType)
            {
                case PacketType.Connection:
                    var p = packet as ConnectionPacket;
                    var client = connectedClients.FirstOrDefault(c => c.ID == clientSocket.ClientID);
                    client.Name = string.Format("{0}#{1}", p.Name, client.ID);
                    var response = new ConnectedPacket(client.ID);
                    var responseBytes = this._serializer.ToBytes(response);
                    client.ClientSocket.Send(responseBytes);
                    this.logger.Log(LogLevel.Info, "New client with name: {0}", client.Name);
                    break;
				case PacketType.GroupMessage:
		            var groupMessagePacket = packet as GroupMessagePacket;
					this.logger.Log(LogLevel.Info, "{0} said to group with id {1}: {2}", 
						groupMessagePacket.SenderName, groupMessagePacket.ReceiverId, groupMessagePacket.Message);
		            break;
            }
        }
    }
}
