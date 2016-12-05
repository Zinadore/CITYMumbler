using System;
using System.Net;
using System.Reactive.Subjects;
using CITYMumbler.Common.Contracts.Services.Logger;
using CITYMumbler.Networking.Contracts;
using CITYMumbler.Networking.Contracts.Serialization;
using CITYMumbler.Networking.Serialization;
using CITYMumbler.Networking.Sockets;
using Splat;
using ILogger = CITYMumbler.Common.Contracts.Services.Logger.ILogger;
using LogLevel = CITYMumbler.Common.Contracts.Services.Logger.LogLevel;

namespace CITYMumbler.Client
{
    public class MumblerClient
    {
        #region Private Members
        private TcpSocket socket;
        private ILogger logger;
        private string username;
        private PacketSerializer serializer;
	    private UserService _userService;
        #endregion

        #region Events
        public EventHandler OnConnected;
        public EventHandler OnDisconnected;
        #endregion

        #region Properties
        public BehaviorSubject<bool> Connected { get; set; }
        #endregion

        public MumblerClient()
        {
            this.socket = new TcpSocket();
            setSocketEvents();
            this.serializer = new PacketSerializer();
            this.Connected = new BehaviorSubject<bool>(false);
            this.logger = Locator.Current.GetService<ILoggerService>().GetLogger(this.GetType());
			this.serializer = new PacketSerializer();
	        this._userService = Locator.Current.GetService<UserService>();
        }

        public void Connect(string host, int port, string username)
        {
            this.username = username;
            this.socket.ConnectAsync(host, port);
        }

        public void Connect(IPAddress address, int port, string username)
        {
            this.username = username;
            this.socket.ConnectAsync(address, port);
        }

        public void Connect(IPEndPoint endpoint, string username)
        {
            this.username = username;
            this.socket.ConnectAsync(endpoint);
        }
        public void SendUserName(string message)
        {
            //    if (socket.Connected)
            //    {
            //        var bytes = System.Text.Encoding.ASCII.GetBytes(message);
            //        this.socket.Send(bytes);
            //    }
        }

	    public void SendGroupMessage(ushort groupId, string message)
	    {
			GroupMessagePacket packet = new GroupMessagePacket(_userService.Me.ID, groupId, _userService.Me.Name, message);
			socket.Send(serializer.ToBytes(packet));
		}

        #region Helpers

        private void setSocketEvents()
        {
            this.socket.OnConnectEnd += Socket_OnConnectEnd;
            this.socket.OnDataReceived += Socket_OnDataReceived;
            //this.socket.OnDisconnected += Socket_OnDisconnected;
        }
        #endregion




        #region Socket Events
        private void Socket_OnDataReceived(object sender, TcpSocketDataReceivedEventArgs e)
        {
            IPacket receivedPacket = this.serializer.FromBytes(e.Payload);
            this.handlePacket(receivedPacket);
        }



        private void Socket_OnConnectEnd(object sender, TcpSocketConnectionStateEventArgs e)
        {
            if (e.Connected)
            {
                this.Connected.OnNext(true);
                this.logger.Log(LogLevel.Info, "Connected successfully");
                this.socket.OnDisconnected += Socket_OnDisconnected;
                var p = new ConnectionPacket(this.username);
                this.socket.Send(this.serializer.ToBytes(p));
                this.OnConnected?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                this.Connected.OnNext(false);
                this.logger.Log(LogLevel.Error, "Failed to connect to server.\r\nError: {0}", e.Exception.Message);
            }
        }

        private void Socket_OnDisconnected(object sender, TcpSocketDisconnectedEventArgs e)
        {
            this.Connected.OnNext(false);
            this.logger.Log(LogLevel.Warn, "Server disconnected");
            this.OnDisconnected?.Invoke(this, EventArgs.Empty);
        }
        #endregion


        private void handlePacket(IPacket receivedPacket)
        {
            switch (receivedPacket.PacketType)
            {
                case PacketType.Connected:
                    var p = receivedPacket as ConnectedPacket;
                    this.logger.Log(LogLevel.Info, "I received my new id and it is: {0}", p.ClientId);
					this._userService.SetMe(new Client(p.ClientId, username));
                    break;
            }
        }

    }
}
