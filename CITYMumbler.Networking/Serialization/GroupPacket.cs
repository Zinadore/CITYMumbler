using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class GroupPacket : Packet
	{
		public ushort Id { get; private set; }
		public string Name { get; private set; }
		public ushort OwnerId { get; private set; }
		public JoinGroupPermissionTypes PermissionType { get; private set; }
		public byte TimeThreshold { get; private set; }
		public ushort[] UserList { get; private set; }

		public GroupPacket(string name, ushort id, ushort ownerId, JoinGroupPermissionTypes permissionType, byte timeThreshold, ushort[] userList = null)
		{
			PacketType = PacketType.GroupPacket;
			Id = id;
			Name = name;
			OwnerId = ownerId;
			this.PermissionType = permissionType;
			TimeThreshold = timeThreshold;
			UserList = userList;
		}

		public byte GetNoOfUsers() { return (byte) UserList.Length; }
	}
}
