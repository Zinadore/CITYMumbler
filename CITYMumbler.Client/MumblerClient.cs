using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using CITYMumbler.Networking.Contracts.Serialization;
using CITYMumbler.Networking.Serialization;
using CITYMumbler.Networking.Sockets;

namespace CITYMumbler.Client
{
    public class MumblerClient
    {
        private TcpSocket socket;

        public MumblerClient()
        {
            this.socket = new TcpSocket();
            this.socket.OnConnectEnd += (s, e) =>
            {
                if (e.Connected)
                {
                    socket.OnDisconnected += (ds, de) =>
					{ 
                        Console.WriteLine("Connection with server lost");
                    };
					//TODO Send here the client login credentials
                    Console.WriteLine("Connected successfuly");
                } else
                {
                    Console.WriteLine(e.Exception.Message);
                }
            };


            this.socket.OnDataReceived += Socket_OnDataReceived;
        }

        public void Send(string message)
        {
            var bytes = System.Text.Encoding.ASCII.GetBytes(message);
            this.socket.Send(bytes);
        }

        private void Socket_OnDataReceived(object sender, TcpSocketDataReceivedEventArgs e)
        {
            string message = System.Text.Encoding.UTF32.GetString(e.Payload);
            Console.WriteLine(message);
        }

        public void Connect()
        {
			this.socket.ConnectAsync(IPAddress.Loopback, 21992);
        }
    }
}
