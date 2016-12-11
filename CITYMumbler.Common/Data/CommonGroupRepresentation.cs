using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Common.Data
{
	/// <summary>
	/// Used a common traslation between the client side and server side representation of the group.
	/// </summary>
	public class CommonGroupRepresentation
    {
		/// <summary>
		/// the id of the group
		/// </summary>
        public ushort ID { get; set; }
		/// <summary>
		/// The name of the group
		/// </summary>
        public string Name { get; set; }
		/// <summary>
		/// The id of the owner of the group
		/// </summary>
        public ushort OwnerID { get; set; }
		/// <summary>
		/// The Join policy of the group. can be either free (anyone can join), password (asks for a password), of permission (asks for permission by the group owner)
		/// </summary>
		public JoinGroupPermissionTypes PermissionType { get; set; }
		/// <summary>
		/// The time that a user is allowed to stay inactive before being kicked. In minutes. Example: a value of 10 will set the threshold to 10 minutes.
		/// </summary>
		public byte TimeoutThreshold { get; set; }
		/// <summary>
		/// The list of the ids of the users in this group
		/// </summary>
        public ushort[] UserIdList { get; set; }
    }
}
