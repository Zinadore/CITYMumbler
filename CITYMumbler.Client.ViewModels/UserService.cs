using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace CITYMumbler.Client.ViewModels
{
	class UserService
	{
		public ReactiveList<Client> UserList;
		public Client Me;

		public void AddClient(ushort id, string name)
		{
			Client client = new Client(id, name);
			UserList.Add(client);
		}

		public void RemoveClient(ushort id)
		{
			foreach (var client in UserList)
			{
				if (client.ID == id)
					UserList.Remove(client);
			}
		}

		public Client GetClient(ushort id)
		{
			foreach (var client in UserList)
			{
				if (client.ID == id)
					return client;
			}
			throw new InstanceNotFoundException();
		}
	}
}
