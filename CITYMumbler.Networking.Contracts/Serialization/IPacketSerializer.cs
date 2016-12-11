using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITYMumbler.Networking.Contracts.Serialization
{
	/// <summary>
	/// The interface that a PacketSerializer should implement
	/// </summary>
	public interface IPacketSerializer
	{
		byte[] ToBytes(IPacket packet);
		IPacket FromBytes(byte[] bytes);
	}
}
