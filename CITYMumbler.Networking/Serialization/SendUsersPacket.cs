using CITYMumbler.Common.Data;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// sent by the server in response to a RequestSendUserssPacket. 
	/// Contains all the clients in the system in a list of users.
	/// </summary>
	public class SendUsersPacket : Packet
	{
		/// <summary>
		/// Constructor of the class.
		/// </summary>
		/// <param name="userList">A list of CommonClientRepresentations</param>
		public SendUsersPacket(CommonClientRepresentation[] userList)
		{
			PacketType = PacketType.SendUsers;
			UserList = userList;
		}

		public CommonClientRepresentation[] UserList { get; private set; }

		/// <summary>
		/// Returns the total number of users in the system.
		/// </summary>
		/// <returns>Returs the array of the UsersList.</returns>
		public byte GetNoOfUsers() { return (byte) UserList.Length; }
	}
}
