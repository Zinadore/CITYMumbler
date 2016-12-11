using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent from the client when the owner of a group has given ownership over to a different user.
	/// </summary>
	public class ChangeGroupOwnerPacket : Packet
	{
		public ushort ClientId { get; private set; }
		public ushort GroupId { get; private set; }
		public ushort NewOwnerId { get; private set; }

		/// <summary>
		/// The Constructor of the class.
		/// </summary>
		/// <param name="clientId">The id of the sender. It should be the owner of the group. Checks are performed server-side.</param>
		/// <param name="groupId">The id of the group whose ownership is to change.</param>
		/// <param name="newOwnerId">The id of the new owner.</param>
		public ChangeGroupOwnerPacket(ushort clientId, ushort groupId, ushort newOwnerId)
		{
			this.PacketType = Contracts.PacketType.ChangeGroupOwner;
			this.ClientId = clientId;
			this.GroupId = groupId;
			this.NewOwnerId = newOwnerId;
		}
	}
}
