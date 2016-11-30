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
        public bool Running { get; private set; }

        //The port the listener is currently running on (if running)
        public int Port { get; private set; }

        public event EventHandler<OnAcceptedTcpSocketEventArgs> OnAccepted;


        public TcpSocketListener(int port)
        {
            this.Port = port;
        }

        public void Start()
        {
            if (!this.Running)
            {
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
                this.listener.Close();
                this.listener = null;
                this.Running = false;
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
                // Store the REMOTE socket into a variable. This is the socket representing the CLIENT here.
                var accepted = this.listener.EndAccept(ar);
                // Tell the listener to start Accepting connections again. We do it here so it can accept while the delegate is running.
                this.listener.BeginAccept(BeginAccept_Callback, null);
                // Fire the event to all the subscribers.
                this.OnAccepted?.Invoke(this, new OnAcceptedTcpSocketEventArgs(accepted));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
        #endregion

    }
}
