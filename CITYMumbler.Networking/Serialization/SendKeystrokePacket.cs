using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class SendKeystrokePacket : PrivateMessagePacket
	{
		public SendKeystrokePacket(ushort senderId, ushort receiverId, string message) : base(senderId, receiverId, message)
		{
			this.PacketType = Contracts.PacketType.SendKeystroke;
		}
	}
}
