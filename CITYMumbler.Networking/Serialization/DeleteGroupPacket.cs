using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by the client in order to delete a group.
	/// </summary>
	public class DeleteGroupPacket : Packet
	{
		public ushort ClientId { get; private set; }
		public ushort GroupId { get; private set; }

		/// <summary>
		/// Constructor for the class.
		/// </summary>
		/// <param name="clientId">The ID of the client requesting the deletion.</param>
		/// <param name="groupId">The ID of the group to be deleted.</param>
		public DeleteGroupPacket(ushort clientId, ushort groupId)
		{
			this.PacketType = PacketType.DeleteGroup;
			this.ClientId = clientId;
			this.GroupId = groupId;
		}
	}
}
