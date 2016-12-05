using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Data;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class SendGroupsPacket : Packet
	{
		public SendGroupsPacket(Group[] groupList)
		{
			PacketType = PacketType.SendGroups;
			GroupList = groupList;
		}

		public Group[] GroupList { get; private set; }

		public byte GetNoOfGroups() { return (byte) GroupList.Length; }
	}
}
