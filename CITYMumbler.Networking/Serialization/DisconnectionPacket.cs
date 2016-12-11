using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by the client in order to disconnect from the system.
	/// </summary>
	public class DisconnectionPacket : Packet
	{
		public ushort ClientId { get; private set; }

		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="ClientId">The ID of the client that wants to disconnect.</param>
		public DisconnectionPacket(ushort ClientId)
		{
			this.PacketType = PacketType.Disconnection;
			this.ClientId = ClientId;
		}
	}
}
