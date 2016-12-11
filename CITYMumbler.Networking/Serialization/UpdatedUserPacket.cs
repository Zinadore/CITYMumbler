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
	/// Sent by the server to all the clients when a change has occured in the list of users. 
	/// The possible reasons are: a user has been CREATED, a user has been DELETED.
	/// The UpdateUserType attribute indicates the action.
	/// </summary>
	public class UpdatedUserPacket : Packet
	{
		public UpdatedUserType UpdateAction { get; private set; }
		public CommonClientRepresentation Client { get; private set; }
		public ushort UserId { get; private set; }

		/// <summary>
		/// Constructor of the class for when a new user has been CREATED. Takes the user that was created.
		/// </summary>
		/// <param name="updateAction">The UpdateAction of the packet. Should be UpdatedUserType.Created.</param>
		/// <param name="client">The user that was created</param>
		public UpdatedUserPacket(UpdatedUserType updateAction, CommonClientRepresentation client)
		{
			PacketType = PacketType.UpdatedUser;
			if (updateAction != UpdatedUserType.Created)
				throw new ArgumentException("UpdateAction should be CREATED, if the second parameter is a client");

			UpdateAction = updateAction;
			Client = client;
		}

		/// <summary>
		/// Constructor of the class for when a new user has been DELETED. Takes the id of the user that
		/// was deleted.
		/// </summary>
		/// <param name="updateAction">The UpdateAction of the packet. Should be UpdatedUserType.Deleted</param>
		/// <param name="userId">The ID of the user that was deleted</param>
		public UpdatedUserPacket(UpdatedUserType updateAction, ushort userId)
		{
			PacketType = PacketType.UpdatedUser;
			if (updateAction != UpdatedUserType.Deleted)
				throw new ArgumentException("UpdateAction should be DELETED, if the second parameter is an id");

			UpdateAction = updateAction;
			UserId = userId;
		}
	}
}
