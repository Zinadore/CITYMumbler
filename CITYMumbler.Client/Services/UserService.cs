using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace CITYMumbler.Client
{
	public class UserService
	{
		public ReactiveList<Client> UserList { get; private set; }
		public Client Me { get; private set; }

		public UserService()
		{
			UserList = new ReactiveList<Client>();
		}

		public void SetMe(Client me) { this.Me = me; }

		public void AddUser(Client client)
		{
			UserList.Add(client);
		}

		public void RemoveUser(Client newClient)
		{
		    UserList.Remove(newClient);
		}
	}
}
