using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITYMumbler.Networking.Contracts.Serialization
{
	public interface IPacket
	{
		PacketType PacketType { get; }
	}
}
