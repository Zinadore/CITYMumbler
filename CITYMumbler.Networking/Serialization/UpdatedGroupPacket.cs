using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Data;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by the server to all the clients when a change has occured in the list of groups. 
	/// The possible reasons are: a group has been CREATED, a group has been DELETED, 
	/// a user has JOINED a GROUP, a user has LEFT a GROUP. The UpdateGroupType attribute indicates
	/// the action.
	/// </summary>
	public class UpdatedGroupPacket : Packet
	{
		public UpdatedGroupType UpdateAction { get; private set; }
		public GroupPacket GroupPacket { get; private set; }
		public ushort GroupId { get; private set; }
		public ushort UserId { get; private set; }

		/// <summary>
		/// Constructor of the class for when a new group has been CREATED. Takes the group that was created
		/// and creates a GroupPacket for it.
		/// </summary>
		/// <param name="updateAction">The UpdateAction of the packet. Should be UpdatedGroupType.Created.</param>
		/// <param name="group">The group that was created</param>
		public UpdatedGroupPacket(UpdatedGroupType updateAction, CommonGroupRepresentation group)
		{
			PacketType = PacketType.UpdatedGroup;
			if (updateAction != UpdatedGroupType.Created)
				throw new ArgumentException("UpdateAction should be CREATED, if the second parameter is a group");

			UpdateAction = updateAction;
			GroupPacket = new GroupPacket(group.Name, group.ID, group.OwnerID, group.PermissionType, group.TimeoutThreshold, group.UserIdList);
		}

		/// <summary>
		/// Constructor of the class for when a new group has been CREATED. Takes a GroupPacket
		/// </summary>
		/// <param name="updateAction">The UpdateAction of the packet. Should be UpdatedGroupType.Created</param>
		/// <param name="groupPacket">The group that was created</param>
		public UpdatedGroupPacket(UpdatedGroupType updateAction, GroupPacket groupPacket)
		{
			PacketType = PacketType.UpdatedGroup;
			if (updateAction != UpdatedGroupType.Created)
				throw new ArgumentException("UpdateAction should be CREATED, if the second parameter is a group");

			UpdateAction = updateAction;
			GroupPacket = groupPacket;
		}

		/// <summary>
		/// Constructor of the class for when a new group has been DELETED. Takes the id of the group that
		/// was deleted.
		/// </summary>
		/// <param name="updateAction">The UpdateAction of the packet. Should be UpdatedGroupType.Deleted</param>
		/// <param name="groupId">The ID of the group that was deleted</param>
		public UpdatedGroupPacket(UpdatedGroupType updateAction, ushort groupId)
		{
			PacketType = PacketType.UpdatedGroup;
			if (updateAction != UpdatedGroupType.Deleted)
				throw new ArgumentException("UpdateAction should be DELETED, if the second parameter is an id");

			UpdateAction = updateAction;
			GroupId = groupId;
		}

		/// <summary>
		/// Constructor of the class for when a user has JOINED or LEFT a group. Takes the ID of the user that
		/// has joined/left, and the ID of the group.
		/// </summary>
		/// <param name="updateAction">The UpdateAction of the packet. Should be UpdatedGroupType.UserJoined or UpdatedGroupType.UserLeft</param>
		/// <param name="userId">The ID of the user that has joined/left</param>
		/// <param name="groupId">The ID of the group that was joined/left.</param>
		public UpdatedGroupPacket(UpdatedGroupType updateAction, ushort userId, ushort groupId)
		{
			PacketType = PacketType.UpdatedGroup;
			if (updateAction == UpdatedGroupType.UserJoined || updateAction == UpdatedGroupType.UserLeft)
			{
				UpdateAction = updateAction;
				GroupId = groupId;
				UserId = userId;
			}
			else
			{
				throw new ArgumentException("UpdateAction should be USERJOINED or USERLEFT, the other parameters are a user id and a group id");
			}
		}
	}
}
