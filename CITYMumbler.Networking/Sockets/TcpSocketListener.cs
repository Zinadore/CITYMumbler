using System;
using System.Net;
using System.Net.Sockets;

namespace CITYMumbler.Networking.Sockets
{
    public sealed class TcpSocketListener
    {
        // This socket represents THIS class on the communication
        private Socket listener;

        // Tells if the listener is running or not
        public bool Running { get; private set; } = false;

        //The port the listener is currently running on (if running)
        public int Port { get; private set; }

        public event EventHandler<OnAcceptedTcpSocketEventArgs> OnAccepted;

        public TcpSocketListener() { this.Port = default(int); }
        public TcpSocketListener(int port)
        {
            this.Port = port;
        }

        public void Start(int port)
        {
            if (!this.Running)
            {
                this.Port = port;
                this.listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.listener.Bind(new IPEndPoint(IPAddress.Any, this.Port));
                this.listener.Listen(70);
                this.listener.BeginAccept(BeginAccept_Callback, null);
                this.Running = true;
            }
            else
            {
                throw new InvalidOperationException("TcpSocketListener is already running");
            }
        }

        public void Stop()
        {
            if (this.Running)
            {
                this.Running = false;
                this.listener.Close();
                this.listener = null;
            }
            else
            {
                throw new InvalidOperationException("TcpSocketListener is not running");
            }
        }


        #region Async Callbacks
        private void BeginAccept_Callback(IAsyncResult ar)
        {
            try
            {
                if (!Running) return;
                // Store the REMOTE socket into a variable. This is the socket representing the CLIENT here.
                var accepted = this.listener.EndAccept(ar);
                // Tell the listener to start Accepting connections again. We do it here so it can accept while the delegate is running.
                this.listener.BeginAccept(BeginAccept_Callback, null);
                // Fire the event to all the subscribers.
                this.OnAccepted?.Invoke(this, new OnAcceptedTcpSocketEventArgs(accepted));
            }
            catch (Exception ex)
            {
                if (ex is SocketException)
                {
                    Console.Write(((SocketException)ex).ErrorCode);
                }
                Console.WriteLine(ex.StackTrace);
            }
        }
        #endregion

    }
}
