using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class ConnectionPacket : Packet
	{
		public string Name { get; private set; }


		public ConnectionPacket(string name)
		{
			this.PacketType = Contracts.PacketType.Connection;
			this.Name = name;
		}
	}
}
