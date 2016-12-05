using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Data;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class SendUsersPacket : Packet
	{
		public SendUsersPacket(Client[] userList)
		{
			PacketType = PacketType.SendUsers;
			UserList = userList;
		}

		public Client[] UserList { get; private set; }

		public byte GetNoOfUsers() { return (byte) UserList.Length; }
	}
}
