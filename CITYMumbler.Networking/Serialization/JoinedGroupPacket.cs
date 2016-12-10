using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class JoinedGroupPacket : Packet
	{
		public ushort ClientId { get; private set; }
		public ushort GroupId { get; private set; }
		public ushort[] Users { get; private set; }

		public JoinedGroupPacket(ushort ClientId, ushort GroupId, ushort[] userIds)
		{
			this.PacketType = PacketType.JoinedGroup;
			this.ClientId = ClientId;
			this.GroupId = GroupId;
			this.Users = userIds;
		}

		public byte GetNoOfUsers() { return (byte) Users.Length; }
	}
}
