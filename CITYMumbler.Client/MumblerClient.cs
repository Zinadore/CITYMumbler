using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Reactive.Subjects;
using CITYMumbler.Common.Contracts.Services.Logger;
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
        private TcpSocket socket;
        private ILogger logger;
        private string username;
        private PacketSerializer pSerializer;
        public BehaviorSubject<bool> Connected { get; set; }
        

        public MumblerClient()
        {
            this.socket = new TcpSocket();
            setSocketEvents();
            this.pSerializer = new PacketSerializer();
            this.Connected = new BehaviorSubject<bool>(false);
            this.logger = Locator.Current.GetService<ILoggerService>().GetLogger(this.GetType());
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
            this.socket.OnDisconnected += Socket_OnDisconnected;
        }
        #endregion




        #region Socket Events
        private void Socket_OnDataReceived(object sender, TcpSocketDataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void Socket_OnConnectEnd(object sender, TcpSocketConnectionStateEventArgs e)
        {
            if (e.Connected)
            {
                this.Connected.OnNext(true);
                this.logger.Log(LogLevel.Info, "Connected successfully");
                var p = new RegisterClientPacket(this.username);
                this.socket.Send(this.pSerializer.ToBytes(p));
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
        }
        #endregion
       

       
    }
}
