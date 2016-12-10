using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by both the server and the client to send the keystroke of user for live typing. Inherits the PrivateMessagePacket.
	/// </summary>
	public class SendKeystrokePacket : PrivateMessagePacket
	{
		/// <summary>
		/// The constructor of the class. 
		/// </summary>
		/// <param name="senderId">The id of the sender user</param>
		/// <param name="receiverId">The id of the reciever user</param>
		/// <param name="senderName">The name of the sender user. Included for performance reasons, to avoid searchig for their name.</param>
		/// <param name="message"> The keystroke sent. Does not have a character limit.</param>
		public SendKeystrokePacket(ushort senderId, ushort receiverId, string senderName, string message) : base(senderId, receiverId, senderName, message)
		{
			this.PacketType = Contracts.PacketType.SendKeystroke;
		}
	}
}
