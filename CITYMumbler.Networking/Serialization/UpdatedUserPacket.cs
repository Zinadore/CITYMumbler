using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Data;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class UpdatedUserPacket : Packet
	{
		public UpdatedUserType UpdateAction { get; private set; }
		public CommonClientRepresentation Client { get; private set; }
		public ushort UserId { get; private set; }

		// User Create
		public UpdatedUserPacket(UpdatedUserType updateAction, CommonClientRepresentation client)
		{
			PacketType = PacketType.UpdatedUser;
			if (updateAction != UpdatedUserType.Created)
				throw new ArgumentException("UpdateAction should be CREATED, if the second parameter is a client");

			UpdateAction = updateAction;
			Client = client;
		}

		// User Delete
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
