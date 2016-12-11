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
    public class MumblerClient
    {
        #region Private Members
        private readonly TcpSocket _socket;
        private readonly ILogger _logger;
        private readonly PacketSerializer _serializer;
        private Client _me;
        #endregion

        #region Events
        public EventHandler OnConnected;
        public EventHandler OnDisconnected;
        public EventHandler OnGroupsReceived;
        public EventHandler OnUsersReceived;
        #endregion

        #region Properties
        public string Name => this._me.Name;
        public ushort ID => this._me.ID;
        public BehaviorSubject<bool> Connected { get; set; }
        public ReplaySubject<ChatEntry> GroupMessages{ get; private set; }
        public ReplaySubject<ChatEntry> PrivateMessages { get; private set; }
        public ReactiveList<Group> Groups { get; private set; }
        public ReactiveList<Group> JoinedGroups { get; private set; }
        public ReactiveList<Client> ConnectedUsers { get; private set; }
        public ReactiveList<PrivateChat> PrivateChats { get; private set; }
        #endregion

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

        public void Connect(string host, int port, string username)
        {
            this._me.Name = username;
            this._socket.ConnectAsync(host, port);
        }

        public void Connect(IPAddress address, int port, string username)
        {
            this._me.Name = username;
            this._socket.ConnectAsync(address, port);
        }

        public void Connect(IPEndPoint endpoint, string username)
        {
            this._me.Name = username;
            this._socket.ConnectAsync(endpoint);
        }

	    public void SendGroupMessage(ushort groupId, string message)
	    {
			GroupMessagePacket packet = new GroupMessagePacket(this._me.ID, groupId, this._me.Name, message);
			this._socket.Send(this._serializer.ToBytes(packet));
		}

        public void SendPrivateMessage(ushort recipientId, string message)
        {
            PrivateMessagePacket packet = new PrivateMessagePacket(this._me.ID, recipientId, this._me.Name, message);
            this._socket.Send(this._serializer.ToBytes(packet));
            var entry = new ChatEntry(this._me.ID, this._me.Name, message, 0, recipientId);
            this.PrivateMessages.OnNext(entry);
        }

        public void JoinGroup(ushort groupId)
        {
            JoinGroupPacket packet = new JoinGroupPacket(this._me.ID, groupId);
            this._socket.Send(this._serializer.ToBytes(packet));
        }

        public void JoinGroup(ushort groupId, string password)
        {
            JoinGroupPacket packet = new JoinGroupPacket(this._me.ID, groupId, password);
            this._socket.Send((this._serializer.ToBytes(packet)));
        }

        public void LeaveGroup(ushort groupId)
        {
            lock (this.JoinedGroups)
            {
                this.JoinedGroups.Remove(this.JoinedGroups.FirstOrDefault(g => g.ID == groupId));
            }
            LeaveGroupPacket packet = new LeaveGroupPacket(this._me.ID, groupId);
            this._socket.Send(this._serializer.ToBytes(packet));
        }

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

        public void CloseWhisper(ushort whisperId)
        {
            lock (this.PrivateChats)
            {
                this.PrivateChats.Remove(this.PrivateChats.First(pc => pc.RemoteUser.ID == whisperId));
            }
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
            }
        }
        #region Packet Handlers

        private void handleUpdatedUserPacket(IPacket packet)
        {
            var p = packet as UpdatedUserPacket;
            Client client;

            if (p.UpdateAction == UpdatedUserType.Created)
            {
                client = new Client() { ID = p.Client.ID, Name = p.Client.Name};
                lock (this.ConnectedUsers)
                {
                    if (!this.ConnectedUsers.Contains(client))
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
                    joinedGroup.GroupUsers.Add(user);
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
