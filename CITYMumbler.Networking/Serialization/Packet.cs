using CITYMumbler.Networking.Contracts;
using CITYMumbler.Networking.Contracts.Serialization;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// The superclass that all packets descend from. Has a PacketType.
	/// </summary>
	public class Packet : IPacket
	{
		public PacketType PacketType { get; protected set; }
	}
}
