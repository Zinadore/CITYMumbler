using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Sockets;

namespace CITYMumbler.Server
{
    class Client
    {
        public ushort ID { get; private set; }
        public string Name { get; set; }
        public TcpSocket ClientSocket { get; private set; }
        public IPEndPoint RemoteEndpoint { get; private set; }

        public Client(ushort id, TcpSocket clientSocket, IPEndPoint endPoint)
        {
            this.ID = id;
            this.ClientSocket = clientSocket;
            this.RemoteEndpoint = endPoint;
            this.ClientSocket.ClientID = this.ID;
        }
    }
}
