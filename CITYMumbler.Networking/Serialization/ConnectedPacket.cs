using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class ConnectedPacket : Packet
	{
		public ushort ClientId { get; private set; }

		public ConnectedPacket(ushort clientId)
		{
			this.PacketType = PacketTypeHeader.Connected;
			this.ClientId = clientId;
		}
	}
}
