using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by the client in order to request all the groups. 
	/// The server will respond with a SendGroupsPacket.
	/// Usually sent after the user has logged in.
	/// </summary>
	public class RequestSendGroupsPacket : Packet
	{
		/// <summary>
		/// The constructor of the class.
		/// </summary>
		public RequestSendGroupsPacket()
		{
			PacketType = PacketType.RequestSendGroups;
		}
	}
}
