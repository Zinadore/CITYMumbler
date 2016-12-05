using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITYMumbler.Client
{
	public class Message
	{

		public ushort SenderId { get; private set; }
		public ushort ReceiverId { get; private set; }
		public string SenderName { get; private set; }
		public string Content { get; private set; }


		public Message(ushort senderId, ushort receiverId, string senderName, string message)
		{
			SenderId = senderId;
			ReceiverId = receiverId;
			SenderName = senderName;
			this.Content = message;
		}
	}
}
