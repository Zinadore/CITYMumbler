using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace CITYMumbler.Client
{
	public class ChatService
	{
		public ReactiveList<Chat> ChatList { get; private set; }

		public void AddChat(Group group)
		{
			GroupChat chat = new GroupChat(group);
			ChatList.Add(chat);
		}

		public void AddChat(Client client)
		{
			PrivateChat chat = new PrivateChat(client);
			ChatList.Add(chat);
		}

		public void removeChat(Chat chat) { ChatList.Remove(chat); }
	}
}
