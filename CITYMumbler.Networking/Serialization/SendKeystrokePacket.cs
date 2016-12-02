using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class SendKeystrokePacket : PrivateMessagePacket
	{
		public SendKeystrokePacket(ushort senderId, ushort receiverId, string senderName, string message) : base(senderId, receiverId, senderName, message)
		{
			this.PacketType = PacketTypeHeader.SendKeystroke;
		}
	}
}
