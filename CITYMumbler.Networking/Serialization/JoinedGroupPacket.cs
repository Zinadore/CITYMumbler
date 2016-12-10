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

		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="ClientId">The ID of the user.</param>
		/// <param name="GroupId">The ID of the group.</param>
		public JoinedGroupPacket(ushort ClientId, ushort GroupId)
		{
			this.PacketType = PacketType.JoinedGroup;
			this.ClientId = ClientId;
			this.GroupId = GroupId;
		}
	}
}
