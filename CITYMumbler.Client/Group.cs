using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;
using ReactiveUI;

namespace CITYMumbler.Client
{
	public class Group
	{
		public ReactiveList<Client> userList { get; private set; }
		public string Name { get; private set; }
		public ushort Id { get; private set; }
		public ushort ownerId { get; private set; }
		public JoinGroupPermissionTypes PermissionType { get; private set; }
		public byte TimeThreshold { get; private set; }
	}
}
