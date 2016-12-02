using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class CreateGroupPacket : Packet
	{
		public ushort ClientId { get; private set; }
		public string GroupName { get; private set; }
		public byte TimeThreshold { get; private set; } // in minutes
		public JoinGroupPermissionTypes PermissionType { get; private set; }
		public string Password { get; private set; }


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

		public bool IsPasswordProtected()
		{
			return this.Password != null;
		}

		public bool isFree()
		{
			return this.PermissionType == JoinGroupPermissionTypes.Free;
		}

		public bool needsPermission()
		{
			return this.PermissionType == JoinGroupPermissionTypes.Permission;
		}
	}
}
