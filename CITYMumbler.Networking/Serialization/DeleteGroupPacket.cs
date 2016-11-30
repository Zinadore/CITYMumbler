using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class DeleteGroupPacket : Packet
	{
		public ushort ClientId { get; private set; }
		public ushort GroupId { get; private set; }

		public DeleteGroupPacket(ushort clientId, ushort groupId)
		{
			this.PacketType = PacketTypeHeader.DeleteGroup;
			this.ClientId = clientId;
			this.GroupId = groupId;
		}
	}
}
