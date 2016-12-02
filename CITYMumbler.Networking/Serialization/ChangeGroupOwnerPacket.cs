using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class ChangeGroupOwnerPacket : Packet
	{
		public ushort ClientId { get; private set; }
		public ushort GroupId { get; private set; }
		public ushort NewOwnerId { get; private set; }

		public ChangeGroupOwnerPacket(ushort clientId, ushort groupId, ushort newOwnerId)
		{
			this.PacketType = Contracts.PacketType.ChangeGroupOwner;
			this.ClientId = clientId;
			this.GroupId = groupId;
			this.NewOwnerId = newOwnerId;
		}
	}
}
