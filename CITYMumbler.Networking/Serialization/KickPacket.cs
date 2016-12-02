using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class KickPacket : Packet
	{
		public ushort ClientId;
		public ushort TargetId;
		public ushort GroupId;


		public KickPacket(ushort clientId, ushort targetId, ushort groupId)
		{
			this.PacketType = Contracts.PacketType.Kick;
			this.ClientId = clientId;
			this.TargetId = targetId;
			this.GroupId = groupId;
		}
	}
}
