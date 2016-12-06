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
		public SendGroupsPacket(CommonGroupRepresentation[] groups)
		{
			PacketType = PacketType.SendGroups;
			GroupList = new GroupPacket[groups.Length];

			for (int i = 0; i < groups.Length; i++)
			{
				GroupPacket packetTemp = new GroupPacket(groups[i].Name, groups[i].ID, groups[i].OwnerID, groups[i].PermissionType, groups[i].TimeoutThreshold, groups[i].UserIdList);
				GroupList[i] = packetTemp;
			}
		}

		public SendGroupsPacket(GroupPacket[] groups)
		{
			PacketType = PacketType.SendGroups;
			GroupList = groups;
		}

		public GroupPacket[] GroupList { get; private set; }

		public byte GetNoOfGroups() { return (byte) GroupList.Length; }
	}
}
