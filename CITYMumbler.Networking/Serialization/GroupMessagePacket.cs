using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by both the client and the server. Contains a message from a user towards a group.
	/// </summary>
	public class GroupMessagePacket : Packet
	{
		public ushort SenderId { get; private set; }
		public ushort GroupID { get; private set; }
		public string SenderName { get; private set; }
		public string Message { get; private set; }

		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="senderId">The ID of the sender.</param>
		/// <param name="groupId">The ID of the group that will receive the message.</param>
		/// <param name="senderName">The name of the sender. Used for performance reasons, to avoid looking up the client name.</param>
		/// <param name="message">The message to be sent. Has no character limit.</param>
		public GroupMessagePacket(ushort senderId, ushort groupId, string senderName, string message)
		{
			this.PacketType = PacketType.GroupMessage;
			this.SenderId = senderId;
			this.GroupID = groupId;
			this.SenderName = senderName;
			this.Message = message;
		}
	}
}
