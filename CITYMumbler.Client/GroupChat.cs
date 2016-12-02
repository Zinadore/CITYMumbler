using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITYMumbler.Client
{
	class GroupChat : Chat
	{
		public Group Group { get; private set; }

		public GroupChat(Group group) { this.Group = group; }
	}
}
