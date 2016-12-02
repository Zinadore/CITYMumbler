using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class GroupMessagePacket : Packet
	{
		public ushort SenderId { get; private set; }
		public ushort ReceiverId { get; private set; }
		public string SenderName { get; private set; }
		public string Message { get; private set; }


		public GroupMessagePacket(ushort senderId, ushort receiverId, string senderName, string message)
		{
			this.PacketType = PacketType.GroupMessage;
			this.SenderId = senderId;
			this.ReceiverId = receiverId;
			this.SenderName = senderName;
			this.Message = message;
		}
	}
}
