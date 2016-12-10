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
	/// sent by the server in response to a RequestSendGroupsPacket. 
	/// Contains all the groups in the system in a list of GroupPackets.
	/// </summary>
	public class SendGroupsPacket : Packet
	{
		/// <summary>
		/// Constructor of the class. Takes a list of CommonGroupRepresentations.
		/// Automatically constructs a GroupPacket for each CommonGroupRepresentation and puts it in the GroupList
		/// </summary>
		/// <param name="groups">A list of groups.</param>
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

		/// <summary>
		/// Constructor of the class. Takes a list of GroupPackets.
		/// </summary>
		/// <param name="groups">A list of GroupPackets.</param>
		public SendGroupsPacket(GroupPacket[] groups)
		{
			PacketType = PacketType.SendGroups;
			GroupList = groups;
		}

		public GroupPacket[] GroupList { get; private set; }

		/// <summary>
		/// Returns the number of groups in the system.
		/// </summary>
		/// <returns>The size of the GroupList</returns>
		public byte GetNoOfGroups() { return (byte) GroupList.Length; }
	}
}
