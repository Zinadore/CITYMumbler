using System.IO;
using CITYMumbler.Networking.Contracts;
using CITYMumbler.Networking.Serialization;

namespace CITYMumbler.Networking.Utilities
{
	/// <summary>
	/// Used by the PacketSerializer to serialize a packet into bytes.
	/// </summary>
    public class PacketWritter: BinaryWriter
    {
        private MemoryStream _memoryStream;

		/// <summary>
		/// The Constructor of the class.
		/// </summary>
        public PacketWritter()
            : base()
        {
            this._memoryStream = new MemoryStream();
            // Set BinaryWriter's OutStream to our memory  stream
            OutStream = _memoryStream;
        }
        
		/// <summary>
		/// Reuturns the bytes that are currently in the MemoryStream
		/// </summary>
		/// <returns>Array of bytes</returns>
        public byte[] GetBytes()
        {
            // Close OutStream
            Close();
            // ToArray will return ONLY the bytes used, not the whole of the buffer, i.e no 0/nulls
            byte[] data = this._memoryStream.ToArray();

            return data;
        }

		/// <summary>
		/// Used to erialize a ConnectionPacket
		/// </summary>
		/// <param name="packet">ConnectionPacket</param>
		public void Write(ConnectionPacket packet)
        {
            Write(packet.Name);
        }

		/// <summary>
		/// Used to erialize a DisconnectionPacket
		/// </summary>
		/// <param name="packet">DisconnectionPacket</param>
		public void Write(DisconnectionPacket packet)
        {
            Write(packet.ClientId);
        }

		/// <summary>
		/// Used to erialize a PrivateMessagePacket
		/// </summary>
		/// <param name="packet">PrivateMessagePacket</param>
		public void Write(PrivateMessagePacket packet)
        {
            Write(packet.SenderId);
            Write(packet.ReceiverId);
			Write(packet.SenderName);
            Write(packet.Message);
        }

		/// <summary>
		/// Used to erialize a SendKeystrokePacket
		/// </summary>
		/// <param name="packet">SendKeystrokePacket</param>
		public void Write(SendKeystrokePacket packet)
        {
            Write((PrivateMessagePacket)packet);
        }

		/// <summary>
		/// Used to erialize a GroupMessagePacket
		/// </summary>
		/// <param name="packet">GroupMessagePacket</param>
		public void Write(GroupMessagePacket packet)
        {
            Write(packet.SenderId);
            Write(packet.GroupID);
			Write(packet.SenderName);
            Write(packet.Message);
        }

		/// <summary>
		/// Used to erialize a JoinGroupPacket
		/// </summary>
		/// <param name="packet">JoinGroupPacket</param>
		public void Write(JoinGroupPacket packet)
        {
            Write(packet.CliendId);
            Write(packet.GroupId);

            if (packet.IsPasswordProtected())
                Write(packet.Password);
        }

		/// <summary>
		/// Used to erialize a JoinedGroupPacket
		/// </summary>
		/// <param name="packet">JoinedGroupPacket</param>
		public void Write(JoinedGroupPacket packet)
        {
            Write(packet.ClientId);
            Write(packet.GroupId);
			Write(packet.GetNoOfUsers());

	        if (packet.GetNoOfUsers() > 0)
	        {
		        foreach (var userId in packet.Users)
		        {
			        Write(userId);
		        }
	        }
        }

		/// <summary>
		/// Used to erialize a DeleteGroupPacket
		/// </summary>
		/// <param name="packet">DeleteGroupPacket</param>
		public void Write(DeleteGroupPacket packet)
        {
            Write(packet.ClientId);
            Write(packet.GroupId);
        }

		/// <summary>
		/// Used to erialize a ChangeGroupOwnerPacket
		/// </summary>
		/// <param name="packet">ChangeGroupOwnerPacket</param>
		public void Write(ChangeGroupOwnerPacket packet)
        {
            Write(packet.ClientId);
            Write(packet.GroupId);
            Write(packet.NewOwnerId);
        }

		/// <summary>
		/// Used to erialize a LeaveGroupPacket
		/// </summary>
		/// <param name="packet">LeaveGroupPacket</param>
		public void Write(LeaveGroupPacket packet)
        {
            Write(packet.ClientId);
            Write(packet.GroupId);
        }

		/// <summary>
		/// Used to erialize a LeftGroupPacket
		/// </summary>
		/// <param name="packet">LeftGroupPacket</param>
		public void Write(LeftGroupPacket packet)
        {
            Write(packet.ClientId);
            Write(packet.GroupId);
            Write((byte)packet.LeftGroupType);
        }

		/// <summary>
		/// Used to erialize a ConnectedPacket
		/// </summary>
		/// <param name="packet">ConnectedPacket</param>
		public void Write(ConnectedPacket packet)
        {
            Write(packet.ClientId);
        }

		/// <summary>
		/// Used to erialize a KickPacket
		/// </summary>
		/// <param name="packet">KickPacket</param>
		public void Write(KickPacket packet)
        {
            Write(packet.ClientId);
            Write(packet.TargetId);
            Write(packet.GroupId);
        }

		/// <summary>
		/// Used to erialize a CreateGroupPacket
		/// </summary>
		/// <param name="packet">CreateGroupPacket</param>
		public void Write(CreateGroupPacket packet)
        {
            Write(packet.ClientId);
            Write(packet.GroupName);
            Write(packet.TimeThreshold);
            Write((byte)packet.PermissionType);

            if (packet.PermissionType == JoinGroupPermissionTypes.Password)
                Write(packet.Password);
        }

		/// <summary>
		/// Used to erialize a SendGroupsPacket
		/// </summary>
		/// <param name="packet">SendGroupsPacket</param>
		public void Write(SendGroupsPacket packet)
	    {
		    Write(packet.GetNoOfGroups());
		    foreach (var group in packet.GroupList)
		    {
			    Write(group.Id);
			    Write(group.Name);
			    Write(group.OwnerId);
				Write((byte) group.PermissionType);
				Write(group.TimeThreshold);
		    }
	    }

		/// <summary>
		/// Used to erialize a SendUsersPacket
		/// </summary>
		/// <param name="packet">SendUsersPacket</param>
		public void Write(SendUsersPacket packet)
		{
			Write(packet.GetNoOfUsers());
			foreach (var client in packet.UserList)
			{
				Write(client.ID);
				Write(client.Name);
			}
		}

		/// <summary>
		/// Used to erialize a GroupPacket
		/// </summary>
		/// <param name="packet">GroupPacket</param>
		public void Write(GroupPacket packet)
	    {
		    Write(packet.Id);
			Write(packet.Name);
			Write(packet.OwnerId);
			Write((byte) packet.PermissionType);
			Write(packet.TimeThreshold);
		    byte NoOfUsers = packet.GetNoOfUsers();
			Write(NoOfUsers);

			if (NoOfUsers > 0)
		    {
				foreach (ushort userId in packet.UserList)
				{
					Write(userId);
				}
			}
	    }

		/// <summary>
		/// Used to erialize a RequestGroupPacket
		/// </summary>
		/// <param name="packet">RequestGroupPacket</param>
		public void Write(RequestGroupPacket packet)
	    {
		    Write(packet.GroupId);
	    }

		/// <summary>
		/// Used to erialize a UpdatedGroupPacket
		/// </summary>
		/// <param name="packet">UpdatedGroupPacket</param>
		public void Write(UpdatedGroupPacket packet)
	    {
			Write((byte)packet.UpdateAction);

			switch (packet.UpdateAction)
			{
				case UpdatedGroupType.Created:
					Write(packet.GroupPacket);
					break;
				case UpdatedGroupType.Deleted:
					Write(packet.GroupId);
					break;
				case UpdatedGroupType.UserJoined:
					Write(packet.UserId);
					Write(packet.GroupId);
					break;
				case UpdatedGroupType.UserLeft:
					Write(packet.UserId);
					Write(packet.GroupId);
					break;
			}
		}

		/// <summary>
		/// Used to erialize a UpdatedUserPacket
		/// </summary>
		/// <param name="packet">UpdatedUserPacket</param>
		public void Write(UpdatedUserPacket packet)
	    {
			Write((byte)packet.UpdateAction);

			switch (packet.UpdateAction)
			{
				case UpdatedUserType.Created:
					Write(packet.Client.ID);
					Write(packet.Client.Name);
					break;
				case UpdatedUserType.Deleted:
					Write(packet.UserId);
					break;
			}
		}
	}
}
