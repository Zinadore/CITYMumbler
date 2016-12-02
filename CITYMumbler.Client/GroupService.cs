using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;
using ReactiveUI;

namespace CITYMumbler.Client
{
	class GroupService
	{
		public ReactiveList<Group> GroupList { get; private set; }
		public bool EnableServerMode { get; private set; }

		public GroupService(bool enableServerMode)
		{
			this.EnableServerMode = enableServerMode;


			if (EnableServerMode)
			{
				//TODO Write logic for removing users on group threshold on server
			}
		}

		public void AddGroup(Group group)
		{
			GroupList.Add(group);
		}

		public void RemoveGroup(ushort id)
		{
			foreach (var g in GroupList)
			{
				if (g.Id == id)
				{
					GroupList.Remove(g);
					return;
				}
			}
			throw new InstanceNotFoundException();
		}

		public void RemoveGroup(Group group)
		{
			foreach (var g in GroupList)
			{
				if (g.Id == group.Id)
				{
					GroupList.Remove(group);
					return;
				}
			}
			throw new InstanceNotFoundException();
		}
	}
}
