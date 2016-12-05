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
		public Group(string name, ushort id, ushort ownerId, JoinGroupPermissionTypes permissionType, byte timeThreshold)
		{
			this.UserList = new ReactiveList<Client>();
			Name = name;
			Id = id;
			this.ownerId = ownerId;
			PermissionType = permissionType;
			TimeThreshold = timeThreshold;
		}

		public ReactiveList<Client> UserList { get; private set; }
		public string Name { get; private set; }
		public ushort Id { get; private set; }
		public ushort ownerId { get; private set; }
		public JoinGroupPermissionTypes PermissionType { get; private set; }
		public byte TimeThreshold { get; private set; }
	}
}
