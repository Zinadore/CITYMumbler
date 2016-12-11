using System;
using CITYMumbler.Networking.Contracts;
using ReactiveUI;

namespace CITYMumbler.Server
{
	/// <summary>
	/// The respresentation of the group at the server
	/// </summary>
    internal class Group
    {
		/// <summary>
		/// The id of the group
		/// </summary>
        public ushort ID { get; set; }
		/// <summary>
		/// The name of the group
		/// </summary>
        public string Name { get; set; }
		/// <summary>
		/// the owner id of the group
		/// </summary>
        public ushort OwnerID { get; set; }
		/// <summary>
		/// The Join policy of the group. can be either free (anyone can join), password (asks for a password), of permission (asks for permission by the group owner)
		/// </summary>
		public JoinGroupPermissionTypes PermissionType { get; set; }
		/// <summary>
		/// The time that a user is allowed to stay inactive before being kicked. In minutes. Example: a value of 10 will set the threshold to 10 minutes.
		/// </summary>
		public byte Threshold { get; set; }
		/// <summary>
		/// The password of the group. Only used if the JoinGroupPermissionType is password
		/// </summary>
        public string Password { get; set; }
		/// <summary>
		/// The list of the users in the group
		/// </summary>
        public ReactiveList<Client> Clients { get; set; }
		/// <summary>
		/// The last time the group received a message
		/// </summary>
        public DateTime LastUpdate { get; set; } = DateTime.Now;

    }
}
