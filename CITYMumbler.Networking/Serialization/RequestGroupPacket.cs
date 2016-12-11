using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by the client in order to receive a GroupPacket. Contains all the information about a group.
	/// Usually sent when the client want to join the group.
	/// </summary>
	public class RequestGroupPacket : Packet
	{
		public ushort GroupId { get; private set; }

		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="id">The ID of the group that should be delivered by the server.</param>
		public RequestGroupPacket(ushort id)
		{
			PacketType = PacketType.RequestGroup;
			GroupId = id;
		}
	}
}
