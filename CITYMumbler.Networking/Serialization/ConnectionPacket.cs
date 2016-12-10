using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by the client in order to connect to the server. 
	/// Includes the prefered name of the user.
	/// </summary>
	public class ConnectionPacket : Packet
	{
		public string Name { get; private set; }

		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="name">The name that the user wants to use in the chat.</param>
		public ConnectionPacket(string name)
		{
			this.PacketType = Contracts.PacketType.Connection;
			this.Name = name;
		}
	}
}
