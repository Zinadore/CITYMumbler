using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class PrivateMessagePacket : Packet
	{
		public ushort SenderId { get; private set; }
		public ushort ReceiverId { get; private set; }
		public string Message { get; private set; }


		public PrivateMessagePacket(ushort senderId, ushort receiverId, string message)
		{
			this.PacketType = Contracts.PacketType.PrivateMessage;
			this.SenderId = senderId;
			this.ReceiverId = receiverId;
			this.Message = message;
		}
	}
}
