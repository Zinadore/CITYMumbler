using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by the server when the user has left a group. 
	/// </summary>
	public class LeftGroupPacket : Packet
	{
		public ushort ClientId { get; private set; }
		public ushort GroupId { get; private set; }
		public LeftGroupTypes LeftGroupType { get; private set; }

		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="clientId">The ID of the user that has left the group.</param>
		/// <param name="groupId">The ID of the group.</param>
		/// <param name="leftGroupType">The reason the user has left this group. can be either normal (left of their own free will), kicked (they were kicked by the owner of the group) or TimeOutReached (the user has been inactive for longer that the inactivity threshold of the group allows.)</param>
		public LeftGroupPacket(ushort clientId, ushort groupId, LeftGroupTypes leftGroupType)
		{
			this.PacketType = Contracts.PacketType.LeftGroup;
			this.ClientId = clientId;
			this.GroupId = groupId;
			this.LeftGroupType = leftGroupType;
		}
	}
}
