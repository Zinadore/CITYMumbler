using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by the client in order to kick a user from a group.
	/// </summary>
	public class KickPacket : Packet
	{
		public ushort ClientId;
		public ushort TargetId;
		public ushort GroupId;

		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="clientId">The id of the client making the kick request. Should be the owner of the group.</param>
		/// <param name="targetId">The ID of the client to be kicked.</param>
		/// <param name="groupId">The group ID from which the targetId will be kicked from.</param>
		public KickPacket(ushort clientId, ushort targetId, ushort groupId)
		{
			this.PacketType = Contracts.PacketType.Kick;
			this.ClientId = clientId;
			this.TargetId = targetId;
			this.GroupId = groupId;
		}
	}
}
