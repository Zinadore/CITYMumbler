using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class RegisterClientPacket : Packet
	{
		public string Name { get; private set; }


		public RegisterClientPacket(string name)
		{
			this.PacketType = PacketTypeHeader.Connection;
			this.Name = name;
		}
	}
}
