using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class LeftGroupPacket : Packet
	{
		public ushort ClientId { get; private set; }
		public ushort GroupId { get; private set; }
		public LeftGroupTypes LeftGroupType { get; private set; }

		public LeftGroupPacket(ushort clientId, ushort groupId, LeftGroupTypes leftGroupType)
		{
			this.PacketType = Contracts.PacketType.LeftGroup;
			this.ClientId = clientId;
			this.GroupId = groupId;
			this.LeftGroupType = leftGroupType;
		}
	}
}
