using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Packet Used to send a Private Message. Is sent from both the client and the server.
	/// </summary>
	public class PrivateMessagePacket : Packet
	{
		public ushort SenderId { get; private set; }
		public ushort ReceiverId { get; private set; }
		public string SenderName { get; private set; }
		public string Message { get; private set; }

		/// <summary>
		/// The constructor of the class. 
		/// </summary>
		/// <param name="senderId">The id of the sender user</param>
		/// <param name="receiverId">The id of the reciever user</param>
		/// <param name="senderName">The name of the sender user. Included for performance reasons, to avoid searchig for their name.</param>
		/// <param name="message"> The message sent. Does not have a character limit.</param>
		public PrivateMessagePacket(ushort senderId, ushort receiverId, string senderName, string message)
		{
			this.PacketType = Contracts.PacketType.PrivateMessage;
			this.SenderId = senderId;
			this.ReceiverId = receiverId;
			this.SenderName = senderName;
			this.Message = message;
		}
	}
}
