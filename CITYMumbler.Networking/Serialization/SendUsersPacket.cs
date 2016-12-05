using CITYMumbler.Common.Data;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class SendUsersPacket : Packet
	{
		public SendUsersPacket(CommonClientRepresentation[] userList)
		{
			PacketType = PacketType.SendUsers;
			UserList = userList;
		}

		public CommonClientRepresentation[] UserList { get; private set; }

		public byte GetNoOfUsers() { return (byte) UserList.Length; }
	}
}
