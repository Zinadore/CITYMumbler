using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Mime;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows;
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
        private ushort groupSeed = 1;
        private object seedLock;
        // Log all the things
        private ILogger logger;
        private IPacketSerializer _serializer;
        private Task _groupCleanupTask;
        private CancellationTokenSource _cleanupSource;
        #endregion

        /// <summary>
        /// The port the server is running on. Default value is 21992
        /// </summary>
        public int Port { get; private set; } = 21992;

        /// <summary>
        /// The time after which a group is considered inactive. Default value
        /// </summary>
        public int Threshold { get; private set; } = 60;

        /// <summary>
        /// A behavior subject that emits true while the server is running and false otherwise
        /// </summary>
        public BehaviorSubject<bool> IsRunning { get; private set; }

        /// <summary>
        /// The constructor. It just initializes everything. Nothing begins happening here.
        /// </summary>
        /// <param name="loggerService">A logger service, used to generate logs.</param>
        public MumblerServer(ILoggerService loggerService)
        {
            this.seedLock = new object();
            this._connectedClients = new List<Client>();
            this._groupList= new ReactiveList<Group>();
            this.logger = loggerService.GetLogger(this.GetType());
            this.listener = new TcpSocketListener();
            this._serializer = new PacketSerializer();
            this.IsRunning = new BehaviorSubject<bool>(false);
            this._cleanupSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Starts the server and puts it in listenning mode on the specified port. It also starts the group cleanup task.
        /// </summary>
        /// <param name="port">The port the server will listen on</param>
        /// <param name="threshold">The time after which a group will be considered inactive</param>
        public void Start(int port, int threshold)
        {
            this.Port = port;
            this.Threshold = threshold;
            this.listener.OnAccepted += OnAccepted_Callback;
            this.listener.Start(port);
            this.IsRunning.OnNext(true);
            this._groupCleanupTask = Task.Factory.StartNew(GroupCleanup, TaskCreationOptions.LongRunning, this._cleanupSource.Token);
            this.logger.Log(LogLevel.Info, "Server listening on port: {0}...", this.Port);
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            foreach (var client in _connectedClients)
            {
                client.ClientSocket.Disconnect();
            }
            this._cleanupSource.Cancel();
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
            var updatePacket = new UpdatedUserPacket(UpdatedUserType.Deleted, disconnected.ID);
            var updateBytes = this._serializer.ToBytes(updatePacket);
            foreach(var c in this._connectedClients)
                c.ClientSocket.Send(updateBytes);
        }

        private void GroupCleanup(object token)
        {
            while (!_cleanupSource.Token.IsCancellationRequested)
            {
                foreach (Group g in this._groupList.ToList())
                {
                    var now = DateTime.Now;
                    if ((now - g.LastUpdate) > TimeSpan.FromSeconds(Threshold))
                    {
                        lock (this._groupList)
                        {
                            this._groupList.Remove(g);
                        }
                        this.logger.Log(LogLevel.Debug, "Removed a group");
                        var updatePacket = new UpdatedGroupPacket(UpdatedGroupType.Deleted, g.ID);
                        var updateBytes = this._serializer.ToBytes(updatePacket);
                        foreach (Client c in this._connectedClients)
                        {
                            c.ClientSocket.Send(updateBytes);
                        }
                    }
                }
                Thread.Sleep(5000);
            }
            this.logger.Log(LogLevel.Debug, "Broke out of the while loop");
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
                case PacketType.LeaveGroup:
                    handleLeaveGroupPacket(clientSocket, packet);
                    break;
                case PacketType.CreateGroup:
                    handleCreateGroupPacket(clientSocket, packet);
                    break;
                case PacketType.Kick:
                    handleKickPacket(clientSocket, packet);
                    break;
                case PacketType.ChangeGroupOwner:
                    handleChangeGroupOwnerPacket(clientSocket, packet);
                    break;
            }
        }

        #region Packet Handling

        private void handleChangeGroupOwnerPacket(TcpSocket clientSocket, IPacket receivedPacket)
        {
            var packet = receivedPacket as ChangeGroupOwnerPacket;
            Group group;
            lock (this._groupList)
            {
                group = this._groupList.FirstOrDefault(g => g.ID == packet.GroupId);
            }
            if (group == null || group.OwnerID != packet.ClientId)
                return;
            group.OwnerID = packet.NewOwnerId;
            this.logger.Log(LogLevel.Debug, "Changed group {0} owner from {1} to {2}", group.ID, packet.ClientId, group.OwnerID);
            var updateBytes = this._serializer.ToBytes(packet);
            lock (this._connectedClients)
            {
                foreach (var connectedClient in this._connectedClients)
                {
                    connectedClient.ClientSocket.Send(updateBytes);
                }
            }
        }

        public void handleKickPacket(TcpSocket clientSocket, IPacket receivedPacket)
        {
            var packet = receivedPacket as KickPacket;
            this.logger.Log(LogLevel.Debug, "User {0} wants to kick user {1} from group {2}", packet.ClientId, packet.TargetId, packet.GroupId);
            Client kicker;
            Client kicked;
            Group group;
            // Make sure the kicker exists
            lock (this._connectedClients)
            {
                kicker = this._connectedClients.FirstOrDefault(c => c.ID == packet.ClientId);
            }
            if (kicker == null)
            {
                this.logger.Log(LogLevel.Debug, "Client {0} tried to kick, but client doesn't exist", packet.ClientId);
                return;
            }

            // Make sure the target exists
            lock (this._connectedClients)
            {
                kicked = this._connectedClients.FirstOrDefault(c => c.ID == packet.TargetId);
            }
            if (kicked == null)
            {
                this.logger.Log(LogLevel.Debug, "Client {0} should be kicked, but client doesn't exist", packet.TargetId);
                return;
            }

            // Make sure the group exists
            lock (this._groupList)
            {
                group = this._groupList.FirstOrDefault(g => g.ID == packet.GroupId);
            }
            if (group == null)
            {
                this.logger.Log(LogLevel.Debug, "Tried to kick client from group {0}, but group doesn't exist", packet.GroupId);
                return;
            }
            if (group.OwnerID != kicker.ID)
            {
                this.logger.Log(LogLevel.Debug, "Client {0} tried to kick someone from group {1} while not being the owner.", packet.ClientId, packet.GroupId);
                return;
            }
            if (group.Clients.Contains(kicked))
                group.Clients.Remove(kicked);
            var kickedPacket = new LeftGroupPacket(kicked.ID, group.ID, LeftGroupTypes.Kicked);
            kicked.ClientSocket.Send(this._serializer.ToBytes(kickedPacket));
            
            var updatedPacket = new UpdatedGroupPacket(UpdatedGroupType.UserLeft, kicked.ID, group.ID);
            var updatedBytes = this._serializer.ToBytes(updatedPacket);
            foreach (Client c in group.Clients)
                c.ClientSocket.Send(updatedBytes);
        }

        private void handleCreateGroupPacket(TcpSocket clientSocket, IPacket receivedPacket)
        {
            var packet = receivedPacket as CreateGroupPacket;
            Client client;
            lock (this._connectedClients)
            {
                client = this._connectedClients.FirstOrDefault(c => c.ID == packet.ClientId);
            }
            if (client == null)
            {
                this.logger.Log(LogLevel.Debug, "Client {0} tried to create a group, but client doesn't exist", packet.ClientId);
                return;
            }
            Group newGroup = new Group()
            {
                Name = packet.GroupName,
                PermissionType = packet.PermissionType,
                OwnerID = client.ID,
                Clients = new ReactiveList<Client>()
            };

            lock (seedLock)
            {
                newGroup.ID = groupSeed++;
            }

            if (newGroup.PermissionType == JoinGroupPermissionTypes.Password)
                newGroup.Password = packet.Password;
            newGroup.Clients.Add(client);
            Group existingGroup;
            lock (this._groupList)
            {
                existingGroup = this._groupList.FirstOrDefault(g => g.Name.Equals(newGroup.Name));
            }

            if (existingGroup != null)
                newGroup.Name = $"{newGroup.Name}#{newGroup.ID}";
            lock (this._groupList)
            {
                this._groupList.Add(newGroup);
            }
            var updateGroupPacket = new UpdatedGroupPacket(UpdatedGroupType.Created, new CommonGroupRepresentation() { ID = newGroup.ID, Name = newGroup.Name, OwnerID = newGroup.OwnerID, PermissionType = newGroup.PermissionType, TimeoutThreshold = newGroup.Threshold});
            var updateGroupBytes = this._serializer.ToBytes(updateGroupPacket);
            lock (this._connectedClients)
            {
                foreach(var c in this._connectedClients)
                    c.ClientSocket.Send(updateGroupBytes);
            }
            var joinGroupPacket = new JoinedGroupPacket(client.ID, newGroup.ID, new []{client.ID});
            client.ClientSocket.Send(this._serializer.ToBytes(joinGroupPacket));
        }

        private void handleLeaveGroupPacket(TcpSocket clientSocket, IPacket receivedPacket)
        {
            var leaveGroupPacket = receivedPacket as LeaveGroupPacket;
            Group group;
            lock (this._groupList)
            {
                group = this._groupList.FirstOrDefault(g => g.ID == leaveGroupPacket.GroupId);
            }

            if (group == null)
            {
                this.logger.Log(LogLevel.Debug, "Client {0} tried to leave group {1}, but the group doesn't exist.", leaveGroupPacket.ClientId, leaveGroupPacket.GroupId);
                return;
            }

            Client client;
            lock (group)
            {
                client = group.Clients.FirstOrDefault(c => c.ID == leaveGroupPacket.ClientId);
            }

            if (client == null)
            {
                this.logger.Log(LogLevel.Debug, "Client {0} tried to leave group {1}, but they are not a member of the group.", leaveGroupPacket.ClientId, leaveGroupPacket.GroupId);
                return;
            }
            lock (group)
            {
                this.logger.Log(LogLevel.Debug, "Client {0} left group {1}.", leaveGroupPacket.ClientId, leaveGroupPacket.GroupId);
                group.Clients.Remove(client);
            }
            bool shouldDeleteGroup = false;

            lock (group)
            {
                if (group.Clients.Count == 0)
                    shouldDeleteGroup = true;
            }

            if (shouldDeleteGroup)
            {
                lock (this._groupList)
                {
                    this._groupList.Remove(group);
                }
                var deletedPacket = new UpdatedGroupPacket(UpdatedGroupType.Deleted, group.ID);
                byte[] deletedBytes = this._serializer.ToBytes(deletedPacket);
                foreach (var c2 in this._connectedClients)
                {
                    c2.ClientSocket.Send(deletedBytes);
                }
                return;
            }

            var updatePacket = new UpdatedGroupPacket(UpdatedGroupType.UserLeft, client.ID, group.ID);
            byte[] updateBytes = this._serializer.ToBytes(updatePacket);
            lock (group)
            {
                foreach (var c in group.Clients)
                    c.ClientSocket.Send(updateBytes);
            }

        }

        private void handlePrivateMessagePacket(TcpSocket clientSocket, IPacket receivedPacket)
        {
            var pm = receivedPacket as PrivateMessagePacket;
            Client receiver;
            lock (this._connectedClients)
            {
                receiver = this._connectedClients.FirstOrDefault(c => c.ID == pm.ReceiverId);
            }
            receiver?.ClientSocket.Send(this._serializer.ToBytes(pm));
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
            var updatePacket = new UpdatedUserPacket(UpdatedUserType.Created, new CommonClientRepresentation() { ID = client.ID, Name = client.Name});
            byte[] updateBytes = this._serializer.ToBytes(updatePacket);

            foreach (var c in this._connectedClients)
            {
                c.ClientSocket.Send(updateBytes);
            }
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
            List<ushort> ids = new List<ushort>();
            foreach (Client c in groupToJoin.Clients)
            {
                ids.Add(c.ID);
            }
            var responsePacket = new JoinedGroupPacket(requestingClient.ID, groupToJoin.ID, ids.ToArray());
            this.logger.Log(LogLevel.Info, "Added user {0} to group {1}", requestingClient.Name, groupToJoin.Name);
            clientSocket.Send(this._serializer.ToBytes(responsePacket));
            var updatePacket = new UpdatedGroupPacket(UpdatedGroupType.UserJoined, requestingClient.ID, groupToJoin.ID);
            byte[] updateBytes = this._serializer.ToBytes(updatePacket);
            foreach (var c in groupToJoin.Clients)
            {
                c.ClientSocket.Send(updateBytes);
            }
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
            groupToMessage.LastUpdate = DateTime.Now;
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
