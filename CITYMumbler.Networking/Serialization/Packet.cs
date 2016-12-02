using CITYMumbler.Networking.Contracts;
using CITYMumbler.Networking.Contracts.Serialization;

namespace CITYMumbler.Networking.Serialization
{
	public class Packet : IPacket
	{
		public PacketType PacketType { get; protected set; }
	}
}
