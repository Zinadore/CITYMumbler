using System;
using System.Net;
using System.Reactive.Subjects;
using CITYMumbler.Common.Contracts.Services.Logger;
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
        private PacketSerializer pSerializer;
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
            this.pSerializer = new PacketSerializer();
            this.Connected = new BehaviorSubject<bool>(false);
            this.logger = Locator.Current.GetService<ILoggerService>().GetLogger(this.GetType());
			this.pSerializer = new PacketSerializer();
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
            this.logger.Log(LogLevel.Info, "Received: {0}", e.Payload);
            //Console.WriteLine("Tarara");
        }
        private void Socket_OnConnectEnd(object sender, TcpSocketConnectionStateEventArgs e)
        {
            if (e.Connected)
            {
                this.Connected.OnNext(true);
                this.logger.Log(LogLevel.Info, "Connected successfully");
                this.socket.OnDisconnected += Socket_OnDisconnected;
                var p = new RegisterClientPacket(this.username);
                this.socket.Send(this.pSerializer.ToBytes(p));
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
       

       
    }
}
