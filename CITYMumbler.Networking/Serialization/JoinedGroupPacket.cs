using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by the server after a client has joined a group. 
	/// </summary>
	public class JoinedGroupPacket : Packet
	{
		public ushort ClientId { get; private set; }
		public ushort GroupId { get; private set; }
		public ushort[] Users { get; private set; }

	    /// <summary>
	    /// The constructor of the class.
	    /// </summary>
	    /// <param name="clientId">The ID of the user.</param>
	    /// <param name="groupId">The ID of the group.</param>
	    /// <param name="userIds">An array of userIds currently in the group</param>
	    public JoinedGroupPacket(ushort clientId, ushort groupId, ushort[] userIds)
		{
			this.PacketType = PacketType.JoinedGroup;
			this.ClientId = clientId;
			this.GroupId = groupId;
			this.Users = userIds;
		}

		public byte GetNoOfUsers() { return (byte) Users.Length; }
	}
}
