using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITYMumbler.Client
{
	class PrivateChat : Chat
	{
		public Client Client { get; private set; }

		public PrivateChat(Client client) { this.Client = client; }
	}
}
