using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITYMumbler.Networking.Contracts.Serialization
{
	/// <summary>
	/// The interface the the packet superclass shoud implement.
	/// </summary>
	public interface IPacket
	{
		PacketType PacketType { get; }
	}
}
