using System.Net;
using CITYMumbler.Networking.Sockets;

namespace CITYMumbler.Server
{
	/// <summary>
	/// The representation of a client at the server side
	/// </summary>
    internal class Client
    {
		/// <summary>
		/// The id of the client
		/// </summary>
        public ushort ID { get; set; }
		/// <summary>
		/// The name of the client
		/// </summary>
        public string Name { get; set; }
		/// <summary>
		/// The socket used by the server to communicate with this client
		/// </summary>
        public TcpSocket ClientSocket { get; set; }
		/// <summary>
		/// The ip endpoint
		/// </summary>
        public IPEndPoint RemoteEndpoint { get; set; }

		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="id">The id of the client</param>
		/// <param name="socket">The socket used by the server to communicate with this client </param>
		/// <param name="endpoint">The ip endpoint</param>
		public Client(ushort id, TcpSocket socket, IPEndPoint endpoint)
        {
            this.ID = id;
            this.ClientSocket = socket;
            this.ClientSocket.ClientID = id;
            this.RemoteEndpoint = endpoint;
        }
    }
}
