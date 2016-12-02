using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class DisconnectionPacket : Packet
	{
		public ushort ClientId { get; private set; }

		public DisconnectionPacket(ushort ClientId)
		{
			this.PacketType = PacketType.Disconnection;
			this.ClientId = ClientId;
		}
	}
}
