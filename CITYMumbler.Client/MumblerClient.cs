using System;
using System.Linq;
using System.Net;
using System.Reactive.Subjects;
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
        }

        public void JoinGroup(ushort groupId)
        {
            JoinGroupPacket packet = new JoinGroupPacket(this._me.ID, groupId);
            this._socket.Send(this._serializer.ToBytes(packet));
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
                    var p = receivedPacket as ConnectedPacket;
                    this._me.ID = p.ClientId;
                    var requestGroupsPacket = new RequestSendGroupsPacket();
                    this._socket.Send(this._serializer.ToBytes(requestGroupsPacket));
                    this.OnConnected?.Invoke(this, EventArgs.Empty);
                    break;
                case PacketType.JoinedGroup:
                    var p1 = receivedPacket as JoinedGroupPacket;
                    var joinedGroup = this.Groups.FirstOrDefault(group => group.ID == p1.GroupId);
                    this.JoinedGroups.Add(joinedGroup);
                    break;
                case PacketType.SendGroups:
                    var p2 = receivedPacket as SendGroupsPacket;
                    this.Groups.Clear();
                    foreach (var group in p2.GroupList)
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
                        foreach (var user in ConnectedUsers)
                        {
                            if (group.UserList.Contains(user.ID))
                            {
                                newGroup.GroupUsers.Add(user);
                            }
                        }
                        Groups.Add(newGroup);
                    }
                    this.JoinGroup(1);
                    this.OnGroupsReceived?.Invoke(this, EventArgs.Empty);
                    break;
                case PacketType.GroupMessage:
                    var groupMessage = receivedPacket as GroupMessagePacket;
                    var groupChatEntry = new ChatEntry(groupMessage.SenderId, groupMessage.SenderName, groupMessage.Message, groupMessage.GroupID);
                    this.GroupMessages.OnNext(groupChatEntry);
                    break;
            }
        }

    }
}
