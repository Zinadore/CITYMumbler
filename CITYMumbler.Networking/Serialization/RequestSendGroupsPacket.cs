using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class RequestSendGroupsPacket : Packet
	{
		public RequestSendGroupsPacket()
		{
			PacketType = PacketType.RequestSendGroups;
		}
	}
}
