using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CITYMumbler.Networking.Sockets
{
    public class TcpSocket: IDisposable
    {
        // The maximum size of the buffer, could be used for multiple cycles
        private const int BUFFER_SIZE = 8192;
        // How many bytes from the beggining of the buffer, are used to denote the actuall buffer size
        private const int BUFFER_HEADER_SIZE = 4;

        private byte[] buffer;
        private MemoryStream payloadStream;
        private int payloadSize;

        private object sendLock = new object();

        // This is us
        private Socket socket;

        public event EventHandler<TcpSocketConnectionStateEventArgs> OnConnectEnd;
        //public event EventHandler<TcpSocketConnectionStateEventArgs> OnConnectionError;
        public event EventHandler<TcpSocketDataReceivedEventArgs> OnDataReceived;
        public event EventHandler<TcpSocketDisconnectedEventArgs> OnDisconnected;

        public bool Connected { get; private set; } = false;
        public bool IsDisposed { get; private set; } = false;

        public int ClientID { get; set; } = default(int);

        /// <summary>
        /// Should be used on the client side mainly. Where you need a new socket to communicate.
        /// </summary>
        public TcpSocket() : this(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
        }

        /// <summary>
        /// Should be used server side mainly, when you already have an existing socket (representing a client)
        /// </summary>
        /// <param name="s">The socket to be wrapped</param>
        public TcpSocket(Socket s)
        {
            this.socket = s;
            this.Connected = s.Connected;
            init();
            if (this.Connected)
            {
                this.beginRead();
            }
        }

        ~TcpSocket() { Dispose(); }
        private void init()
        {
            this.buffer = new byte[BUFFER_SIZE];
        }

        #region Connection Methods
        public void Connect(string host, int port)
        {
            this.socket.Connect(host, port);
            this.Connected = true;
        }

        public void Connect(IPEndPoint endPoint)
        {
            this.socket.Connect(endPoint);
            this.Connected = true;
        }

        public void Connect(IPAddress address, int port)
        {
            this.socket.Connect(address, port);
            this.Connected = true;
        }

        public void ConnectAsync(string host, int port)
        {
            this.checkDisposed();
            this.socket.BeginConnect(host, port, BeginConnect_Callback, null);
        }

        public void ConnectAsync(IPEndPoint endPoint)
        {
            this.checkDisposed();
            this.socket.BeginConnect(endPoint, BeginConnect_Callback, null);
        }

        public void ConnectAsync(IPAddress address, int port)
        {
            this.checkDisposed();
            this.socket.BeginConnect(address, port, BeginConnect_Callback, null);
        }
        #endregion

        #region Send Methods
        public void Send(byte[] payload)
        {
            this.checkDisposed();
            byte[] headerBuffer = BitConverter.GetBytes(payload.Length);

            byte[] actualBuffer = new byte[headerBuffer.Length + payload.Length];

            Buffer.BlockCopy(headerBuffer, 0, actualBuffer, 0, headerBuffer.Length);
            Buffer.BlockCopy(payload, 0, actualBuffer, headerBuffer.Length, payload.Length);

            this.socket.BeginSend(actualBuffer, 0, actualBuffer.Length, SocketFlags.None, BeginSend_Callback, null);
        }

        public void RapidSend(byte[] payload)
        {
            this.checkDisposed();
            lock (this.sendLock)
            {
                this.socket.Send(BitConverter.GetBytes(payload.Length));
                this.socket.Send(payload);
            }
        }
        #endregion

        private void beginRead()
        { 
            try
            {
                // We only want to read the header here, to see how long our actual payload is
                this.socket.BeginReceive(this.buffer, 0, BUFFER_HEADER_SIZE, SocketFlags.None, BeginReceive_HeaderCallback, null);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        #region CallBacks
        private void BeginConnect_Callback(IAsyncResult ar)
        {
            Exception ex = null;
            try
            {
                this.socket.EndConnect(ar);
                this.Connected = true;
                this.beginRead();
            }
            catch (Exception e)
            {
                ex = e;
                this.Connected = false;
                this.Disconnected();
            }
            finally
            {
                this.OnConnectEnd?.Invoke(this, new TcpSocketConnectionStateEventArgs(ex, Connected));
            }
        }

        private void BeginReceive_HeaderCallback(IAsyncResult ar)
        {
            try
            {
                // How many bytes were read. Usually this should get us the whole header, but just in case.
                var bytesRead = this.socket.EndReceive(ar);
                if (bytesRead <= 0)
                {
                    throw new SocketException((int)SocketError.ConnectionAborted);
                }
                if(bytesRead < BUFFER_HEADER_SIZE)
                {
                    var bytesLeft = BUFFER_HEADER_SIZE - bytesRead;

                    // TODO: Handle this shit better in case we are under attack !
                    while(socket.Available < bytesLeft)
                    {
                        Thread.Sleep(100);
                    }

                    // At this point the rest of the header should be here. We are expecting up to 4K of data so a sync receive should be ok
                    this.socket.Receive(this.buffer, bytesRead, bytesLeft, SocketFlags.None);
                }
                this.payloadSize = BitConverter.ToInt32(this.buffer, 0);
                this.payloadStream = new MemoryStream();
                //Array.Clear(this.buffer, 0, this.buffer.Length);

                // In case we need to receive LESS that our buffer size (i.e small packet or w/e) we do NOT want to receive up to BUFFER_SIZE
                // reason being that if some more data arrives (say from the next packet or message or anything) we don't want them clamped up together
                // beacause that will cause garbage data on what we are currently receiving and make the next stream of data incomplete
                int actualSizeToReceive = (this.payloadSize > BUFFER_SIZE) ? BUFFER_SIZE : this.payloadSize;

                this.socket.BeginReceive(this.buffer, 0, actualSizeToReceive, SocketFlags.None, BeginReceive_PayloadCallback, null);
            }
            catch(Exception e)
            {
                Disconnected();
                Debug.Print(e.StackTrace);
            }
        }

        private void BeginReceive_PayloadCallback(IAsyncResult ar)
        {
            try
            {
                var readBytes = this.socket.EndReceive(ar);

                if (readBytes <= 0)
                {
                    throw new SocketException((int)SocketError.ConnectionAborted);
                }

                this.payloadSize -= readBytes;

                this.payloadStream.Write(this.buffer, 0, readBytes);
                //Array.Clear(this.buffer, 0, this.buffer.Length);

                if (this.payloadSize > 0)
                {
                    int remainingBytesToReceive = (this.payloadSize > BUFFER_SIZE) ? BUFFER_SIZE : this.payloadSize;
                    this.socket.BeginReceive(this.buffer, 0, remainingBytesToReceive, SocketFlags.None, BeginReceive_PayloadCallback, null);
                }
                else
                {
                    this.payloadStream.Close();
                    // ToArray will return only the written bytes
                    byte[] payload = payloadStream.ToArray();
                    this.payloadStream = null;
                    // Start receiving again before we raise the event, in case events take long to fire/finish
                    beginRead();
                    this.OnDataReceived?.Invoke(this, new TcpSocketDataReceivedEventArgs(payload));
                }

            }
            catch(Exception e)
            {
                Debug.Print(e.StackTrace);
                Disconnected();
            }
        }

        private void BeginSend_Callback(IAsyncResult ar)
        {
            try
            {
                this.socket.EndSend(ar);
            }
            catch(Exception e)
            {
                Disconnected();
                Debug.Print(e.Message);
                Debug.Print(e.StackTrace);
            }
        }
        #endregion

        #region Disconnection
        public void Disconnect()
        {
            this.checkDisposed();
            if (this.Connected)
            {
                this.socket.Shutdown(SocketShutdown.Both);
                this.Connected = false;
            }
        }
        private void Disconnected()
        {
            this.Connected = false;
            this.OnDisconnected?.Invoke(this, new TcpSocketDisconnectedEventArgs(this.ClientID));
        }
        #endregion

        #region IDisposable

        private void checkDisposed()
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException("this");
        }
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                this.socket.Close();
                this.socket = null;
                this.buffer = null;
                this.payloadSize = 0;
                this.Connected = false;
            }
        }
        #endregion

    }
}
