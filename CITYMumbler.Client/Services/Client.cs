using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Sockets;

namespace CITYMumbler.Client
{
	public class Client
	{
		public ushort ID { get; private set; }
		public string Name { get; private set; }

		public Client(ushort id, string name)
		{
			this.ID = id;
			this.Name = name;
		}
	}
}
