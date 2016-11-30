using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class JoinedGroupPacket : Packet
	{
		public ushort ClientId { get; private set; }
		public ushort GroupId { get; private set; }

		public JoinedGroupPacket(ushort ClientId, ushort GroupId)
		{
			this.PacketType = PacketTypeHeader.JoinedGroup;
			this.ClientId = ClientId;
			this.GroupId = GroupId;
		}
	}
}
