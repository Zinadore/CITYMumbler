using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using CITYMumbler.Common.Contracts.Services.Logger;
using CITYMumbler.Common.Data;
using CITYMumbler.Networking.Contracts;
using CITYMumbler.Networking.Contracts.Serialization;
using CITYMumbler.Networking.Serialization;
using CITYMumbler.Networking.Sockets;
using ReactiveUI;

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
        private List<Client> _connectedClients;
        private ReactiveList<Group> _groupList;
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
            this._connectedClients = new List<Client>();
            this._groupList= new ReactiveList<Group>();
            this.logger = loggerService.GetLogger(this.GetType());
            this.listener = new TcpSocketListener();
            this._serializer = new PacketSerializer();
            this.IsRunning = new BehaviorSubject<bool>(false);

            this._groupList.Add(new Group()
            {
                Name = "MyGroup",
                ID = 1,
                PermissionType = JoinGroupPermissionTypes.Free,
                OwnerID = 2,
                Clients = new ReactiveList<Client>()
            });
            this._groupList.Add(new Group()
            {
                Name = "MyOtherGroup",
                ID = 2,
                PermissionType = JoinGroupPermissionTypes.Free,
                OwnerID = 2,
                Clients = new ReactiveList<Client>()
            });
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
            foreach (var client in _connectedClients)
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
            lock (this._connectedClients)
            {
                this._connectedClients.Add(newClient);
            }
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
            var disconnected = _connectedClients.FirstOrDefault(client => client.ID == e.ClientID);
            _connectedClients.Remove(disconnected);
            this.logger.Log(LogLevel.Warn, "Connection lost with {0}", disconnected?.RemoteEndpoint);
        }

        private void handlePacket(IPacket packet, TcpSocket clientSocket)
        {
            
            switch (packet.PacketType)
            {
                case PacketType.Connection:
                    handleConnectionPacket(clientSocket, packet);
                    break;
                case PacketType.JoinGroup:
                    handleJoinGroupPacket(clientSocket, packet);
                    break;
				case PacketType.GroupMessage:
		            handleGroupMessagePacket(clientSocket, packet);
		            break;
                case PacketType.PrivateMessage:
                    handlePrivateMessagePacket(clientSocket, packet);
                    break;
                case PacketType.RequestSendGroups:
                    handleRequestSendGroupsPacket(clientSocket, packet);                   
                    break;
                case PacketType.RequestSendUsers:
                    handleRequestSendUsersPacket(clientSocket, packet);
                    break;
            }
        }

        #region Packet Handling

        private void handlePrivateMessagePacket(TcpSocket clientSocket, IPacket receivedPacket)
        {
            var pm = receivedPacket as PrivateMessagePacket;
            Client receiver;
            lock (this._connectedClients)
            {
                receiver = this._connectedClients.FirstOrDefault(c => c.ID == pm.ReceiverId);
            }
            receiver.ClientSocket.Send(this._serializer.ToBytes(pm));
        }

        private void handleConnectionPacket(TcpSocket clientSocket, IPacket receivedPacket)
        {
            var p = receivedPacket as ConnectionPacket;
            Client client;
            lock (this._connectedClients)
            {
                client = _connectedClients.FirstOrDefault(c => c.ID == clientSocket.ClientID);
            }
            client.Name = string.Format("{0}#{1}", p.Name, client.ID);
            var response = new ConnectedPacket(client.ID);
            var responseBytes = this._serializer.ToBytes(response);
            client.ClientSocket.Send(responseBytes);
            this.logger.Log(LogLevel.Info, "New client with name: {0}", client.Name);
        }

        private void handleJoinGroupPacket(TcpSocket clientSocket, IPacket receivedPacket)
        {
            this.logger.Log(LogLevel.Debug, "Received request to join group");
            var joinPacket = receivedPacket as JoinGroupPacket;
            var groupToJoin = this._groupList.FirstOrDefault(group => group.ID == joinPacket.GroupId);
            var requestingClient = this._connectedClients.FirstOrDefault(c => c.ID == joinPacket.CliendId);
            // Client provided wrong password
            if (groupToJoin.PermissionType == JoinGroupPermissionTypes.Password && !groupToJoin.Password.Equals(joinPacket.Password))
            {
                this.logger.Log(LogLevel.Warn, "User {0} tried to join group {1}, but provided wrong password.", requestingClient.Name, groupToJoin.Name);
                //TODO: Should inform client about wrong password
                return;
            }

            // Client is already part of the group
            if (groupToJoin.Clients.Contains(requestingClient))
            {
                this.logger.Log(LogLevel.Warn, "User {0} tried to join group {1} while already being a member.", requestingClient.Name, groupToJoin.Name);
                return;
            }

            //Everything went well. We should add the client to the group
            groupToJoin.Clients.Add(requestingClient);
            var responsePacket = new JoinedGroupPacket(requestingClient.ID, groupToJoin.ID);
            this.logger.Log(LogLevel.Info, "Added user {0} to group {1}", requestingClient.Name, groupToJoin.Name);
            clientSocket.Send(this._serializer.ToBytes(responsePacket));
        }

        private void handleGroupMessagePacket(TcpSocket clientSocket, IPacket receivedPacket)
        {
            var groupMessagePacket = receivedPacket as GroupMessagePacket;
            Group groupToMessage;
            lock (this._groupList)
            {
                groupToMessage = this._groupList.FirstOrDefault(group => group.ID == groupMessagePacket.GroupID);
            }
             
            // Make sure the group exists.
            if (groupToMessage == null)
            {
                this.logger.Log(LogLevel.Warn,"Group with id {0} not found.", groupMessagePacket.GroupID);
                return;
            }
            // Make sure the client is part of the group
            if (groupToMessage.Clients.FirstOrDefault(c => c.ID == groupMessagePacket.SenderId) == null)
            {
                this.logger.Log(LogLevel.Warn, "Client {0} tried to message a group they dont belong to ({1}).", groupMessagePacket.SenderId, groupMessagePacket.GroupID);
                return;
            }

            foreach (var c in groupToMessage.Clients)
            {
                c.ClientSocket.Send(this._serializer.ToBytes(groupMessagePacket));
            }
        }

        private void handleRequestSendGroupsPacket(TcpSocket clientSocket, IPacket receivedPacket)
        {
            this.logger.Log(LogLevel.Debug, "Preparing groups to send");
            List<GroupPacket> groupPackets = new List<GroupPacket>();
            foreach (var group in this._groupList)
            {
                groupPackets.Add(new GroupPacket(group.Name, group.ID, group.OwnerID, group.PermissionType, group.Threshold));
            }
            var requestGroupsResponse = new SendGroupsPacket(groupPackets.ToArray());
            clientSocket.Send(this._serializer.ToBytes(requestGroupsResponse));
        }

        private void handleRequestSendUsersPacket(TcpSocket clientSocket, IPacket receivedPacket)
        {
            this.logger.Log(LogLevel.Debug, "Preparing users to send");
            List<CommonClientRepresentation> clientList = new List<CommonClientRepresentation>();

            lock (this._connectedClients)
            {
                foreach (var clientToSend in this._connectedClients)
                {
                    clientList.Add(new CommonClientRepresentation() { ID = clientToSend.ID, Name = clientToSend.Name });
                }
            }

            var usersResponse = new SendUsersPacket(clientList.ToArray());
            clientSocket.Send(this._serializer.ToBytes(usersResponse));
        }
        #endregion
    }
}
