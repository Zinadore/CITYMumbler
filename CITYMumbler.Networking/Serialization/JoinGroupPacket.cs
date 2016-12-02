using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class JoinGroupPacket : Packet
	{
		public ushort CliendId { get; private set; }
		public ushort GroupId { get; private set; }
		public string Password { get; private set; }


		public JoinGroupPacket(ushort cliendId, ushort groupId, string password = null)
		{
			this.PacketType = Contracts.PacketType.JoinGroup;
			this.CliendId = cliendId;
			this.GroupId = groupId;
			this.Password = password;
		}

		public bool IsPasswordProtected()
		{
			return Password != null;
		}
	}
}
