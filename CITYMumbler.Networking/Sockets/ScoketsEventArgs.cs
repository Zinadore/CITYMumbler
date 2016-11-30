using System;
using System.Net;
using System.Net.Sockets;

namespace CITYMumbler.Networking.Sockets
{
    public class OnAcceptedTcpSocketEventArgs : EventArgs {
        public Socket AcceptedSocket { get; private set; }
        public IPEndPoint RemoteEndpoint { get; private set; }

        public OnAcceptedTcpSocketEventArgs(Socket s)
        {
            this.AcceptedSocket = s;
            this.RemoteEndpoint = (IPEndPoint)s.RemoteEndPoint;
        }
    }

    public class TcpSocketConnectionStateEventArgs: EventArgs
    {
        public Exception Exception { get; private set; }
        public bool Connected { get; private set; }

        public TcpSocketConnectionStateEventArgs(Exception ex, bool connected)
        {
            this.Exception = ex;
            this.Connected = connected;
        }
    }

    public class TcpSocketDataReceivedEventArgs: EventArgs
    {
        public byte[] Payload { get; private set; }

        public TcpSocketDataReceivedEventArgs(byte[] payload)
        {
            this.Payload = payload;
        }
    }
}
