using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class LeaveGroupPacket : Packet
	{
		public ushort ClientId { get; private set; }
		public ushort GroupId { get; private set; }

		public LeaveGroupPacket(ushort clientId, ushort groupId)
		{
			this.PacketType = PacketTypeHeader.LeaveGroup;
			this.ClientId = clientId;
			this.GroupId = groupId;
		}
	}
}
