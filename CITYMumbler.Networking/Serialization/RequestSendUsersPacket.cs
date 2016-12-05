using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class RequestSendUsersPacket : Packet
	{
		public RequestSendUsersPacket() { PacketType = PacketType.RequestSendUsers; }
	}
}

