using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Data;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class UpdatedGroupPacket : Packet
	{
		public UpdatedGroupType UpdateAction { get; private set; }
		public GroupPacket GroupPacket { get; private set; }
		public ushort GroupId { get; private set; }
		public ushort UserId { get; private set; }

		// Group Create
		public UpdatedGroupPacket(UpdatedGroupType updateAction, CommonGroupRepresentation group)
		{
			PacketType = PacketType.UpdatedGroup;
			if (updateAction != UpdatedGroupType.Created)
				throw new ArgumentException("UpdateAction should be CREATED, if the second parameter is a group");

			UpdateAction = updateAction;
			GroupPacket = new GroupPacket(group.Name, group.ID, group.OwnerID, group.PermissionType, group.TimeoutThreshold, group.UserIdList);
		}

		public UpdatedGroupPacket(UpdatedGroupType updateAction, GroupPacket groupPacket)
		{
			PacketType = PacketType.UpdatedGroup;
			if (updateAction != UpdatedGroupType.Created)
				throw new ArgumentException("UpdateAction should be CREATED, if the second parameter is a group");

			UpdateAction = updateAction;
			GroupPacket = groupPacket;
		}

		// Group Delete
		public UpdatedGroupPacket(UpdatedGroupType updateAction, ushort groupId)
		{
			PacketType = PacketType.UpdatedGroup;
			if (updateAction != UpdatedGroupType.Deleted)
				throw new ArgumentException("UpdateAction should be DELETED, if the second parameter is an id");

			UpdateAction = updateAction;
			GroupId = groupId;
		}

		// User Joined and User left
		public UpdatedGroupPacket(UpdatedGroupType updateAction, ushort userId, ushort groupId)
		{
			PacketType = PacketType.UpdatedGroup;
			if (updateAction != UpdatedGroupType.UserJoined || updateAction != UpdatedGroupType.UserLeft)
				throw new ArgumentException("UpdateAction should be USERJOINED or USERLEFT, the other parameters are a user id and a group id");

			UpdateAction = updateAction;
			GroupId = groupId;
			UserId = userId;
		}
	}
}
