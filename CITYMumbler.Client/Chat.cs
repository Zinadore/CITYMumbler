using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace CITYMumbler.Client
{
	public abstract class Chat
	{
		public ReactiveList<Message> messageList { get; private set; }

		public void AddMessage(Message message)
		{
			messageList.Add(message);
		}
	}
}