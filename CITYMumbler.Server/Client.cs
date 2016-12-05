using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Sockets;

namespace CITYMumbler.Server
{
    internal class Client
    {
        public ushort ID { get; set; }
        public string Name { get; set; }
        public TcpSocket ClientSocket { get; set; }
        public IPEndPoint RemoteEndpoint { get; set; }

        public Client(ushort id, TcpSocket socket, IPEndPoint endpoint)
        {
            this.ID = id;
            this.ClientSocket = socket;
            this.ClientSocket.ClientID = id;
            this.RemoteEndpoint = endpoint;
        }
    }
}
