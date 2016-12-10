using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Represents a group. Sent by the server to to the client to populate the list of groups.
	/// </summary>
	public class GroupPacket : Packet
	{
		public ushort Id { get; private set; }
		public string Name { get; private set; }
		public ushort OwnerId { get; private set; }
		public JoinGroupPermissionTypes PermissionType { get; private set; }
		public byte TimeThreshold { get; private set; }
		public ushort[] UserList { get; private set; }

		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="name">The name of the group.</param>
		/// <param name="id">The id of the group.</param>
		/// <param name="ownerId">The ID of the owner of the group.</param>
		/// <param name="timeThreshold">The time that a user is allowed to stay inactive before being kicked. In minutes. Example: a value of 10 will set the threshold to 10 minutes.</param>
		/// <param name="permissionType">The Join policy of the group. can be either free (anyone can join), password (asks for a password), of permission (asks for permission by the group owner)</param>
		/// <param name="userList">A list of the ids of all the Users that are in the group.</param>
		public GroupPacket(string name, ushort id, ushort ownerId, JoinGroupPermissionTypes permissionType, byte timeThreshold, ushort[] userList = null)
		{
			PacketType = PacketType.GroupPacket;
			Id = id;
			Name = name;
			OwnerId = ownerId;
			this.PermissionType = permissionType;
			TimeThreshold = timeThreshold;
			UserList = userList ?? new ushort[1];
		}

		/// <summary>
		/// Returns the number of student that are currently in the group.
		/// </summary>
		/// <returns>The number of users in the UserList.</returns>
		public byte GetNoOfUsers() { return (byte) UserList.Length; }
	}
}
