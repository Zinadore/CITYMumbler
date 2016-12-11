using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by the client in order to request all the users in the system. 
	/// The server will respond with a SendUsersPackaget.
	/// Usually sent after the user has logged in.
	/// </summary>
	public class RequestSendUsersPacket : Packet
	{
		/// <summary>
		/// The constructor of the class.
		/// </summary>
		public RequestSendUsersPacket() { PacketType = PacketType.RequestSendUsers; }
	}
}

