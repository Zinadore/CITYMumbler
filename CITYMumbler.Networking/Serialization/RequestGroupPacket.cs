using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class RequestGroupPacket : Packet
	{
		public ushort GroupId { get; private set; }


		public RequestGroupPacket(ushort id)
		{
			PacketType = PacketType.RequestGroup;
			GroupId = id;
		}
	}
}
