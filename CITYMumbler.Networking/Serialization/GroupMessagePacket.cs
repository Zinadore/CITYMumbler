using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	public class GroupMessagePacket : Packet
	{
		public ushort SenderId { get; private set; }
		public ushort GroupID { get; private set; }
		public string SenderName { get; private set; }
		public string Message { get; private set; }


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
