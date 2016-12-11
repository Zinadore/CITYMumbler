using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by the server when a client has successfully connected to the server. 
	/// Includes the client's assigned ID.
	/// </summary>
	public class ConnectedPacket : Packet
	{
		public ushort ClientId { get; private set; }

		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="clientId">The newly assigned ID by the server.</param>
		public ConnectedPacket(ushort clientId)
		{
			this.PacketType = PacketType.Connected;
			this.ClientId = clientId;
		}
	}
}
