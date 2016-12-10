using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by the client in order to create a new group.
	/// </summary>
	public class CreateGroupPacket : Packet
	{
		public ushort ClientId { get; private set; }
		public string GroupName { get; private set; }
		public byte TimeThreshold { get; private set; } // in minutes
		public JoinGroupPermissionTypes PermissionType { get; private set; }
		public string Password { get; private set; }

		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="clientId">The id of the client creating the group. It will get assigned as the onwerID of the group.</param>
		/// <param name="groupName">The name of the group.</param>
		/// <param name="timeThreshold">The time that a user is allowed to stay inactive before being kicked. In minutes. Example: a value of 10 will set the threshold to 10 minutes.</param>
		/// <param name="permissionType">The Join policy of the group. can be either free (anyone can join), password (asks for a password), of permission (asks for permission by the group owner)</param>
		/// <param name="password">OPTIONAL. the password of the group. Only needed if permission type is set to password.</param>
		public CreateGroupPacket(ushort clientId, string groupName, byte timeThreshold,
			JoinGroupPermissionTypes permissionType, string password = null)
		{
			this.PacketType = PacketType.CreateGroup;
			this.ClientId = clientId;
			this.GroupName = groupName;
			this.TimeThreshold = timeThreshold;
			this.PermissionType = permissionType;
			this.Password = password;
		}

		/// <summary>
		/// Checks whether the group requires a password to join.
		/// </summary>
		/// <returns>True if group is password protected, False if it is free or requires permisison.</returns>
		public bool IsPasswordProtected()
		{
			return this.Password != null;
		}

		/// <summary>
		/// Checks whether the group is free to join.
		/// </summary>
		/// <returns>True if group is free to join, False if it is password protected or requires permisison.</returns>
		public bool isFree()
		{
			return this.PermissionType == JoinGroupPermissionTypes.Free;
		}

		/// <summary>
		/// Checks whether the group requires permission by the owner in order to join.
		/// </summary>
		/// <returns>True if group needs permission, False if it is free or is password protected.</returns>
		public bool needsPermission()
		{
			return this.PermissionType == JoinGroupPermissionTypes.Permission;
		}
	}
}
