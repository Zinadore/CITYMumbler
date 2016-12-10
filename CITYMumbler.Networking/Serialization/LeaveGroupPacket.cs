using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by the client in order to leave a group. 
	/// </summary>
	public class LeaveGroupPacket : Packet
	{
		public ushort ClientId { get; private set; }
		public ushort GroupId { get; private set; }

		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="clientId">The id of the client that want to leave the group.</param>
		/// <param name="groupId">The ID of the group.</param>
		public LeaveGroupPacket(ushort clientId, ushort groupId)
		{
			this.PacketType = Contracts.PacketType.LeaveGroup;
			this.ClientId = clientId;
			this.GroupId = groupId;
		}
	}
}
