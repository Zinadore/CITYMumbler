using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using CITYMumbler.Common.Contracts.Services.Logger;
using CITYMumbler.Networking.Contracts;
using CITYMumbler.Networking.Contracts.Serialization;
using CITYMumbler.Networking.Serialization;
using CITYMumbler.Networking.Sockets;
using ReactiveUI;
using Splat;
using ILogger = CITYMumbler.Common.Contracts.Services.Logger.ILogger;
using LogLevel = CITYMumbler.Common.Contracts.Services.Logger.LogLevel;

namespace CITYMumbler.Client
{
    /// <summary>
    /// The class that holds all of the business logic related to the client.
    /// </summary>
    public class MumblerClient
    {
        #region Private Members
        private readonly TcpSocket _socket;
        private readonly ILogger _logger;
        private readonly PacketSerializer _serializer;
        private Client _me;
        #endregion

        #region Events
        /// <summary>
        /// Fires once the connection has been successful, and the server has provided the client with an ID
        /// </summary>
        public EventHandler OnConnected;
        //public EventHandler OnDisconnected;
        /// <summary>
        /// Fires when the server has returned a list of all the currently available groups
        /// </summary>
        public EventHandler OnGroupsReceived;

        /// <summary>
        /// Fires once the server has returned a list of all the currently connected users
        /// </summary>
        public EventHandler OnUsersReceived;
        #endregion

        #region Properties
        /// <summary>
        /// The name associated with the current client
        /// </summary>
        public string Name => this._me.Name;

        /// <summary>
        /// The id associated with the current client
        /// </summary>
        public ushort ID => this._me.ID;

        /// <summary>
        /// A behavior subject that emits true while the client is connected to the server. False otherwise
        /// </summary>
        public BehaviorSubject<bool> Connected { get; set; }

        /// <summary>
        /// A replay subject that contains all the group messages received
        /// </summary>
        public ReplaySubject<ChatEntry> GroupMessages{ get; private set; }

        /// <summary>
        /// A replay subject that contains all the private messages received
        /// </summary>
        public ReplaySubject<ChatEntry> PrivateMessages { get; private set; }

        /// <summary>
        /// A reactive list, containing all the groups known to the client
        /// </summary>
        public ReactiveList<Group> Groups { get; private set; }

        /// <summary>
        /// A reactive list containing all the known groups that this client has joined
        /// </summary>
        public ReactiveList<Group> JoinedGroups { get; private set; }

        /// <summary>
        /// A reactivelist containing all the known connected users
        /// </summary>
        public ReactiveList<Client> ConnectedUsers { get; private set; }

        /// <summary>
        /// A reactive list that containes all the private conversations for this client. NOTE: It does not keep the messages
        /// of the communication
        /// </summary>
        public ReactiveList<PrivateChat> PrivateChats { get; private set; }
        #endregion

        /// <summary>
        /// The default constructor. Sets up everything.
        /// </summary>
        public MumblerClient()
        {
            this._socket = new TcpSocket();
            setSocketEvents();
            this._serializer = new PacketSerializer();
            this.Connected = new BehaviorSubject<bool>(false);
            this._logger = Locator.Current.GetService<ILoggerService>().GetLogger(this.GetType());
			this._serializer = new PacketSerializer();
            this._me = new Client();
            this.GroupMessages = new ReplaySubject<ChatEntry>();
            this.PrivateMessages = new ReplaySubject<ChatEntry>();
            this.Groups = new ReactiveList<Group>();
            this.JoinedGroups = new ReactiveList<Group>();
            this.ConnectedUsers = new ReactiveList<Client>();
            this.PrivateChats = new ReactiveList<PrivateChat>();
        }

        /// <summary>
        /// Connects to the server using the provided parameters. The connection is asynchronous, not blocking.
        /// </summary>
        /// <param name="host">The IP address of the server</param>
        /// <param name="port">Port the server is listening on</param>
        /// <param name="username">The username that should be associated with this client</param>
        public void Connect(string host, int port, string username)
        {
            this._me.Name = username;
            this._socket.ConnectAsync(host, port);
        }

        /// <summary>
        /// Connects to the server using the provided parameters. The connection is asynchronous, not blocking.
        /// </summary>
        /// <param name="address">The IP address of the server</param>
        /// <param name="port">The port the server is listening on</param>
        /// <param name="username">The username that should be associated with this client</param>
        public void Connect(IPAddress address, int port, string username)
        {
            this._me.Name = username;
            this._socket.ConnectAsync(address, port);
        }

        /// <summary>
        /// Connects to the server using the provided parameters. The connection is asynchronous, not blocking.
        /// </summary>
        /// <param name="endpoint">The IPEndPoint of the server</param>
        /// <param name="username">The username that should be associated with this client</param>
        public void Connect(IPEndPoint endpoint, string username)
        {
            this._me.Name = username;
            this._socket.ConnectAsync(endpoint);
        }

        /// <summary>
        /// Sends a message to the server, addressed to the specified group
        /// </summary>
        /// <param name="groupId">The id of the group the message is addressed to</param>
        /// <param name="message">The message</param>
	    public void SendGroupMessage(ushort groupId, string message)
	    {
			GroupMessagePacket packet = new GroupMessagePacket(this._me.ID, groupId, this._me.Name, message);
			this._socket.Send(this._serializer.ToBytes(packet));
		}

        /// <summary>
        /// Sends a private message to the server, addressed to the specified user
        /// </summary>
        /// <param name="recipientId">The id of the recipient of the message</param>
        /// <param name="message">The message</param>
        public void SendPrivateMessage(ushort recipientId, string message)
        {
            PrivateMessagePacket packet = new PrivateMessagePacket(this._me.ID, recipientId, this._me.Name, message);
            this._socket.Send(this._serializer.ToBytes(packet));
            var entry = new ChatEntry(this._me.ID, this._me.Name, message, 0, recipientId);
            this.PrivateMessages.OnNext(entry);
        }

        /// <summary>
        /// Notifies the server that the client wants to join the specified group
        /// </summary>
        /// <param name="groupId">The id of the group to join</param>
        public void JoinGroup(ushort groupId)
        {
            JoinGroupPacket packet = new JoinGroupPacket(this._me.ID, groupId);
            this._socket.Send(this._serializer.ToBytes(packet));
        }

        /// <summary>
        /// Notifies the server that the client wants to join the specified group
        /// </summary>
        /// <param name="groupId">The id of the group to join</param>
        /// <param name="password">The password for the group</param>
        public void JoinGroup(ushort groupId, string password)
        {
            JoinGroupPacket packet = new JoinGroupPacket(this._me.ID, groupId, password);
            this._socket.Send((this._serializer.ToBytes(packet)));
        }

        /// <summary>
        /// Leaves the group, and notifies the server
        /// </summary>
        /// <param name="groupId">The id of the group to leave</param>
        public void LeaveGroup(ushort groupId)
        {
            lock (this.JoinedGroups)
            {
                this.JoinedGroups.Remove(this.JoinedGroups.FirstOrDefault(g => g.ID == groupId));
            }
            LeaveGroupPacket packet = new LeaveGroupPacket(this._me.ID, groupId);
            this._socket.Send(this._serializer.ToBytes(packet));
        }

        /// <summary>
        /// Notifies the server that a new group should be created, with the provided info
        /// </summary>
        /// <param name="groupName">The name of the new group</param>
        /// <param name="authenticationType">The type of authentication: None, Password, Permission</param>
        /// <param name="threshold">The time after which a user is consideredinactiveinside the group</param>
        /// <param name="password">The password of the group</param>
        public void CreateGroup(string groupName, JoinGroupPermissionTypes authenticationType, byte threshold,
                                string password = null)
        {
            CreateGroupPacket packet;
            if (authenticationType == JoinGroupPermissionTypes.Password)
            {
                if (password == null)
                    throw new ArgumentNullException(nameof(password));
                packet = new CreateGroupPacket(this._me.ID, groupName, threshold, authenticationType, password);
            }
            else
            {
                packet = new CreateGroupPacket(this._me.ID, groupName, threshold, authenticationType);
            }
            this._socket.Send(this._serializer.ToBytes(packet));
        }


        /// <summary>
        /// Notifies the server that a group's owner should be changed
        /// </summary>
        /// <param name="groupId">The id of the group to be modified</param>
        /// <param name="newOwnerId">The id of the new owner</param>
        public void ChangeGroupOwner(ushort groupId, ushort newOwnerId)
        {
            var packet = new ChangeGroupOwnerPacket(this._me.ID, groupId, newOwnerId);
            this._socket.Send(this._serializer.ToBytes(packet));
        }


        /// <summary>
        /// Creates a private conversation with the specified user. This is only client-side
        /// </summary>
        /// <param name="whisperId">The id of the user to whisper</param>
        public void Whisper(ushort whisperId)
        {
            PrivateChat chat;
            lock (this.PrivateChats)
            {
                chat = this.PrivateChats.FirstOrDefault(pc => pc.RemoteUser.ID == whisperId);
            }

            if (chat != null) return;
            Client receiver;
            lock (this.ConnectedUsers)
            {
                receiver = ConnectedUsers.FirstOrDefault(c => c.ID == whisperId);
            }
            if (receiver == null)
            {
                this._logger.Log(LogLevel.Error, "Tried to whisper a non existing user");
                return;
            }
            lock (this.PrivateChats)
            {
                this.PrivateChats.Add(new PrivateChat(this._me, receiver));
            }
        }


        /// <summary>
        /// Closes the private conversation with the specified user. This is only client-side.
        /// </summary>
        /// <param name="whisperId"></param>
        public void CloseWhisper(ushort whisperId)
        {
            lock (this.PrivateChats)
            {
                this.PrivateChats.Remove(this.PrivateChats.First(pc => pc.RemoteUser.ID == whisperId));
            }
        }

        /// <summary>
        /// Notifies the server that a user should be kicked from the group. The validity of the request is verified server-side
        /// </summary>
        /// <param name="groupId">The id of the group the user should be removed from</param>
        /// <param name="remoteId">The id of the user that should be removed</param>
        public void Kick(ushort groupId, ushort remoteId)
        {
            if (remoteId == this._me.ID)
            {
                this._logger.Log(LogLevel.Debug, "I just tried to kick myself out of a group!");
                return;
            }
            
            var kickPacket = new KickPacket(this._me.ID, remoteId, groupId);
            this._socket.Send(this._serializer.ToBytes(kickPacket));
        }

        #region Helpers

        private void setSocketEvents()
        {
            this._socket.OnConnectEnd += Socket_OnConnectEnd;
            this._socket.OnDataReceived += Socket_OnDataReceived;
        }
        #endregion

        #region Socket Events
        private void Socket_OnDataReceived(object sender, TcpSocketDataReceivedEventArgs e)
        {
            IPacket receivedPacket = this._serializer.FromBytes(e.Payload);
            this.handlePacket(receivedPacket);
        }



        private void Socket_OnConnectEnd(object sender, TcpSocketConnectionStateEventArgs e)
        {
            if (e.Connected)
            {
                this.Connected.OnNext(true);
                this._logger.Log(LogLevel.Info, "Connected successfully");
                this._socket.OnDisconnected += Socket_OnDisconnected;
                var p = new ConnectionPacket(this._me.Name);
                this._socket.Send(this._serializer.ToBytes(p));
            }
            else
            {
                this.Connected.OnNext(false);
                this._logger.Log(LogLevel.Error, "Failed to connect to server.\r\nError: {0}", e.Exception.Message);
            }
        }

        private void Socket_OnDisconnected(object sender, TcpSocketDisconnectedEventArgs e)
        {
            this.Connected.OnNext(false);
            this._logger.Log(LogLevel.Warn, "Server disconnected");
            this.OnDisconnected?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        private void handlePacket(IPacket receivedPacket)
        {
            switch (receivedPacket.PacketType)
            {
                case PacketType.Connected:
                    handleConnectedPacket(receivedPacket);
                    break;
                case PacketType.JoinedGroup:
                    handleJoinedGroupPacket(receivedPacket);
                    break;
                case PacketType.SendGroups:
                    handleSendGroupsPacket(receivedPacket);
                    break;
                case PacketType.GroupMessage:
                    handleGroupMessagePacket(receivedPacket);
                    break;
                case PacketType.SendUsers:
                    handleSendUsersPacket(receivedPacket);
                    break;
                case PacketType.PrivateMessage:
                    handlePrivateMessagePacket(receivedPacket);
                    break;
                case PacketType.UpdatedGroup:
                    handleUpdatedGroupPacket(receivedPacket);
                    break;
                case PacketType.UpdatedUser:
                    handleUpdatedUserPacket(receivedPacket);
                    break;
                case PacketType.LeftGroup:
                    handleLeftGroupPacket(receivedPacket);
                    break;
                case PacketType.ChangeGroupOwner:
                    handleChangeGroupOwnerPacket(receivedPacket);
                    break;
            }
        }

        #region Packet Handlers
        private void handleChangeGroupOwnerPacket(IPacket packet)
        {
            var p = packet as ChangeGroupOwnerPacket;
            Group group;
            lock (this.Groups)
            {
                group = this.Groups.FirstOrDefault(g => g.ID == p.GroupId);
            }
            if (group == null)
                return;
            group.OwnerID = p.NewOwnerId;
        }
        private void handleLeftGroupPacket(IPacket packet)
        {
            var leftPacket = packet as LeftGroupPacket;
            if (leftPacket.ClientId != this._me.ID)
                return;
            Group group;
            lock (this.JoinedGroups)
            {
                group = this.JoinedGroups.FirstOrDefault(g => g.ID == leftPacket.GroupId);
            }

            if (group == null)
            {
                this._logger.Log(LogLevel.Debug, "I was kicked from a group ({0}) that doesn't exist.", leftPacket.GroupId);
                return;
            }

            lock (this.JoinedGroups)
            {
                JoinedGroups.Remove(group);
            }

            group.GroupUsers = new ReactiveList<Client>();
            this._logger.Log(LogLevel.Warn, "I have been kicked from group {0} :(", group.Name);
            
        }
        private void handleUpdatedUserPacket(IPacket packet)
        {
            var p = packet as UpdatedUserPacket;
            Client client;

            if (p.UpdateAction == UpdatedUserType.Created)
            {
                client = new Client() { ID = p.Client.ID, Name = p.Client.Name};
                Client existing;
                lock (this.ConnectedUsers)
                {
                    existing = this.ConnectedUsers.FirstOrDefault(c => c.ID == client.ID);
                    if (existing == null)
                        this.ConnectedUsers.Add(client);
                }
                return;
            }

            lock (this.ConnectedUsers)
                client = this.ConnectedUsers.FirstOrDefault(c => c.ID == p.UserId);
            if (client == null)
                return;
            lock (this.ConnectedUsers)
                this.ConnectedUsers.Remove(client);

        }
        private void handleUpdatedGroupPacket(IPacket packet)
        {
            var p = packet as UpdatedGroupPacket;
            if (p.UpdateAction == UpdatedGroupType.Created)
            {
                Group existingGroup;
                lock (this.Groups)
                {
                    existingGroup = this.Groups.FirstOrDefault(g => g.ID == p.GroupId);
                }
                if (existingGroup != null)
                    return;
                lock(this.Groups)
                this.Groups.Add(new Group() {
                    ID = p.GroupPacket.Id,
                    Name = p.GroupPacket .Name,
                    OwnerID = p.GroupPacket.OwnerId,
                    PermissionType = p.GroupPacket.PermissionType,
                    TimeoutThreshold = p.GroupPacket.TimeThreshold,
                    GroupUsers = new ReactiveList<Client>()
                });
                return;
            }
            else if (p.UpdateAction == UpdatedGroupType.Deleted)
            {
                Group g;
                lock (this.Groups)
                {
                    g = this.Groups.FirstOrDefault(gr => gr.ID == p.GroupId);
                    this.Groups.Remove(g);
                }

                lock (this.JoinedGroups)
                {
                    this.JoinedGroups.Remove(g);
                }

                return;
            }
            else if (p.UpdateAction == UpdatedGroupType.UserJoined)
            {
                Client user;
                Group group;

                lock (this.ConnectedUsers)
                {
                    user = this.ConnectedUsers.FirstOrDefault(u => u.ID == p.UserId);
                }
                if (user == null)
                {
                    this._logger.Log(LogLevel.Debug, "Tried to add user {0} to group {1}, but user did not exist", p.UserId, p.GroupId);
                    return;
                }

                lock (this.Groups)
                {
                    group = this.Groups.FirstOrDefault(g => g.ID == p.GroupId);
                }

                if (group == null)
                {
                    this._logger.Log(LogLevel.Debug, "Tried to add user {0} to group {1}, but group did not exist", p.UserId, p.GroupId);
                    return;
                }

                lock (group)
                {
                    if (!group.GroupUsers.Contains(user))
                        group.GroupUsers.Add(user);
                }
                return;
            }
            else if (p.UpdateAction == UpdatedGroupType.UserLeft)
            {
                Client client;
                Group group;
                lock (this.ConnectedUsers)
                    client = this.ConnectedUsers.FirstOrDefault(c => c.ID == p.UserId);
                if (client == null)
                {
                    this._logger.Log(LogLevel.Debug, "Tried to remove client {0} from group {1}, but client does not exist.", p.UserId, p.GroupId);
                    return;
                }
                lock (this.Groups)
                    group = this.Groups.FirstOrDefault(g => g.ID == p.GroupId);
                if (group == null)
                {
                    this._logger.Log(LogLevel.Debug, "Tried to remove client {0} from group {1}, but group doesn't exist", p.UserId, p.GroupId);
                    return;
                }

                lock (group)
                    group.GroupUsers.Remove(client);
            }
        }
        private void handleSendUsersPacket(IPacket packet)
        {
            var sendUsersPacket = packet as SendUsersPacket;
            lock (this.ConnectedUsers)
            {
                this.ConnectedUsers.Clear();
            }
            foreach (var sentClient in sendUsersPacket.UserList)
            {
                lock (this.ConnectedUsers)
                {
                    this.ConnectedUsers.Add(new Client() {ID = sentClient.ID, Name = sentClient.Name});
                }
            }
        }
        private void handleGroupMessagePacket(IPacket packet)
        {
            var groupMessage = packet as GroupMessagePacket;
            var groupChatEntry = new ChatEntry(groupMessage.SenderId, groupMessage.SenderName, groupMessage.Message, groupMessage.GroupID);
            this.GroupMessages.OnNext(groupChatEntry);
        }
        private void handleSendGroupsPacket(IPacket packet)
        {
            var sendGroupsPacket = packet as SendGroupsPacket;
            lock(this.Groups)
                this.Groups.Clear();
            foreach (var group in sendGroupsPacket.GroupList)
            {
                var newGroup = new Group()
                {
                    ID = group.Id,
                    OwnerID = group.OwnerId,
                    Name = group.Name,
                    PermissionType = group.PermissionType,
                    TimeoutThreshold = group.TimeThreshold,
                    GroupUsers = new ReactiveList<Client>()
                };
                lock(this.Groups)
                    Groups.Add(newGroup);
            }
            this.OnGroupsReceived?.Invoke(this, EventArgs.Empty);
        }
        private void handleConnectedPacket(IPacket packet)
        {
            var p = packet as ConnectedPacket;
            this._me.ID = p.ClientId;
            var requestGroupsPacket = new RequestSendGroupsPacket();
            this._socket.Send(this._serializer.ToBytes(requestGroupsPacket));
            var requestUsersPacket = new RequestSendUsersPacket();
            this._socket.Send(this._serializer.ToBytes(requestUsersPacket));
            this.OnConnected?.Invoke(this, EventArgs.Empty);
        }
        private void handleJoinedGroupPacket(IPacket packet)
        {
            var p1 = packet as JoinedGroupPacket;
            Group joinedGroup;
            int attempts = 1;
            do
            {
                lock (this.Groups)
                {
                    joinedGroup = this.Groups.FirstOrDefault(group => group.ID == p1.GroupId);
                }

                if (joinedGroup == null)
                {
                    Thread.Sleep(100);
                    attempts++;
                }
            } while (joinedGroup == null || attempts > 10);
            if (joinedGroup == null)
                return;
            lock (this.JoinedGroups)
            {
                this.JoinedGroups.Add(joinedGroup);
            }

            foreach (ushort cid in p1.Users)
            {
                Client user;
                lock (this.ConnectedUsers)
                {
                    user = this.ConnectedUsers.FirstOrDefault(c => c.ID == cid);
                }
                if (user != null)
                {
                    if (!joinedGroup.GroupUsers.Contains(user))
                        joinedGroup.GroupUsers.Add(user);
                }
            }
        }
        private void handlePrivateMessagePacket(IPacket packet)
        {
            var pm = packet as PrivateMessagePacket;
            Whisper(pm.SenderId);
            var entry = new ChatEntry(pm.SenderId, pm.SenderName, pm.Message);
            this.PrivateMessages.OnNext(entry);
        }
        #endregion
    }
}
