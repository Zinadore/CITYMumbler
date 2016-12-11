using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Sent by the client in order to join a group.
	/// </summary>
	public class JoinGroupPacket : Packet
	{
		public ushort CliendId { get; private set; }
		public ushort GroupId { get; private set; }
		public string Password { get; private set; }

		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="cliendId">The ID of the client.</param>
		/// <param name="groupId">The ID of the group.</param>
		/// <param name="password">OPTIONAL. The password that is needed to join the group. Only applicable if the groups join permission is 'password'</param>
		public JoinGroupPacket(ushort cliendId, ushort groupId, string password = null)
		{
			this.PacketType = Contracts.PacketType.JoinGroup;
			this.CliendId = cliendId;
			this.GroupId = groupId;
			this.Password = password;
		}

		/// <summary>
		/// Returns whether the group is password protected.
		/// </summary>
		/// <returns>True if the group is password protected, False if it is free to join.</returns>
		public bool IsPasswordProtected()
		{
			return Password != null;
		}
	}
}
