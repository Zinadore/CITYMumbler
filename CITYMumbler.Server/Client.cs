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
        public int ID { get; set; }
        public string Name { get; set; }
        public TcpSocket ClientSocket { get; set; }
        public IPEndPoint RemoteEndpoint { get; set; }
    }
}
