using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Data;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class GroupPacket : Packet
	{
		public ushort Id { get; private set; }
		public string Name { get; private set; }
		public ushort OwnerId { get; private set; }
		public JoinGroupPermissionTypes permissionType { get; private set; }
		public byte TimeThreshold { get; private set; }
		public ushort[] UserList { get; private set; }

		public GroupPacket(string name, ushort id, ushort ownerId, JoinGroupPermissionTypes permissionType, byte timeThreshold, Client[] userList)
		{
			PacketType = PacketType.GroupPacket;
			Id = id;
			Name = name;
			OwnerId = ownerId;
			this.permissionType = permissionType;
			TimeThreshold = timeThreshold;
			UserList = new ushort[userList.Length];
			for (int i = 0; i < userList.Length; i++)
			{
				UserList[i] = userList[i].ID;
			}
		}

		public GroupPacket(string name, ushort id, ushort ownerId, JoinGroupPermissionTypes permissionType, byte timeThreshold, ushort[] userList)
		{
			PacketType = PacketType.GroupPacket;
			Id = id;
			Name = name;
			OwnerId = ownerId;
			this.permissionType = permissionType;
			TimeThreshold = timeThreshold;
			UserList = userList;
		}

		public byte GetNoOfUsers() { return (byte) UserList.Length; }
	}
}
