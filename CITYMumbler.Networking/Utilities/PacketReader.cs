using System;
using System.IO;
using CITYMumbler.Common;
using CITYMumbler.Common.Data;
using CITYMumbler.Networking.Contracts;
using CITYMumbler.Networking.Contracts.Serialization;
using CITYMumbler.Networking.Serialization;

namespace CITYMumbler.Networking.Utilities
{
	/// <summary>
	/// Used by the PaketSerialzer in order to deserialize a packet. Reads bytes and reaturns packets.
	/// </summary>
    public class PacketReader: BinaryReader
    {
		/// <summary>
		/// The constructor of the class.
		/// </summary>
		/// <param name="data">The array of bytes that is to be serialized.</param>
        public PacketReader(byte[] data) : base(new MemoryStream(data)) {}

		/// <summary>
		/// Used to read a packet. The PackeType parameter indicates the way that the packet should be deserialized.
		/// </summary>
		/// <param name="type">The PacketType of the packet.</param>
		/// <returns>The deserialized packet.</returns>
        public IPacket ReadPacket(PacketType type)
        {
            switch (type)
            {
                case PacketType.Connection: return ReadConnectionMessage();

                case PacketType.Disconnection: return ReadDisconnectionMessage();

                case PacketType.Connected: return ReadConnectedMessage();

                case PacketType.PrivateMessage: return ReadPrivateMessageMessage();

                case PacketType.SendKeystroke: return ReadKeyStrokeMessage();

                case PacketType.GroupMessage: return ReadGroupMessageMessage();

                case PacketType.JoinGroup: return ReadJoinGroupMessage();

                case PacketType.JoinedGroup: return ReadJoinedGroupMessage();

                case PacketType.DeleteGroup: return ReadDeleteGroupMessage();

                case PacketType.ChangeGroupOwner: return ReadChangeGroupOwnerMessage();

                case PacketType.LeaveGroup: return ReadLeaveGroupMessage();

                case PacketType.LeftGroup: return ReadLeftGroupMessage();

                case PacketType.Kick: return ReadKickMessage();

                case PacketType.CreateGroup: return ReadCreateGroupMessage();

				case PacketType.SendGroups: return SendGroupsMessage();

				case PacketType.RequestSendGroups: return (IPacket) new RequestSendGroupsPacket();

				case PacketType.SendUsers: return SendUsersMessage();

				case PacketType.RequestSendUsers: return (IPacket) new RequestSendUsersPacket();

				case PacketType.GroupPacket: return GroupPacketMessage();

				case PacketType.RequestGroup: return RequestGroupMessage();

				case PacketType.UpdatedGroup: return UpdatedGroupMessage();

				case PacketType.UpdatedUser: return UpdatedUserMessage();
					
				default: throw new ArgumentException("The provided PacketType is not valid.");
            }
        }

		/// <summary>
		/// Used to read a ConnectionPacket.
		/// </summary>
		/// <returns>ConnectionPacket</returns>
        private IPacket ReadConnectionMessage()
        {
            string name = ReadString();
            IPacket packet = new ConnectionPacket(name);
            return packet;
        }

		/// <summary>
		/// Used to read a DisconnectionPacket.
		/// </summary>
		/// <returns>DisconnectionPacket</returns>
        private IPacket ReadDisconnectionMessage()
        {
            ushort clientId = ReadUInt16();
            IPacket packet = new DisconnectionPacket(clientId);
            return packet;
        }

		/// <summary>
		/// Used to read a ConnectedPacket.
		/// </summary>
		/// <returns>ConnectedPacket</returns>
        private IPacket ReadConnectedMessage()
        {
            ushort clientId = ReadUInt16();
            IPacket packet = new ConnectedPacket(clientId);
            return packet;
        }

		/// <summary>
		/// Used to read a PrivateMessagePacket
		/// </summary>
		/// <returns>PrivateMessagePacket</returns>
        private IPacket ReadPrivateMessageMessage()
        {
            ushort senderId = ReadUInt16();
            ushort receiverId = ReadUInt16();
	        string senderName = ReadString();
            string message = ReadString();
            IPacket packet = new PrivateMessagePacket(senderId, receiverId, senderName, message);
            return packet;
        }

		/// <summary>
		/// Used to read a SendKeystrokePacket
		/// </summary>
		/// <returns>SendKeystrokePacket</returns>
		private IPacket ReadKeyStrokeMessage()
        {
            ushort senderId = ReadUInt16();
            ushort receiverId = ReadUInt16();
	        string senderName = ReadString();
            string message = ReadString();
            IPacket packet = new SendKeystrokePacket(senderId, receiverId, senderName, message);
            return packet;
        }

		/// <summary>
		/// Used to read a GroupMessagePacket
		/// </summary>
		/// <returns>GroupMessagePacket</returns>
		private IPacket ReadGroupMessageMessage()
        {
            ushort senderId = ReadUInt16();
            ushort receiverId = ReadUInt16();
	        string senderName = ReadString();
            string message = ReadString();
            IPacket packet = new GroupMessagePacket(senderId, receiverId, senderName, message);
            return packet;
        }

		/// <summary>
		/// used to read a JoinGroupPacket
		/// </summary>
		/// <returns>JoinGroupPacket</returns>
		private IPacket ReadJoinGroupMessage()
        {
            ushort cliendId = ReadUInt16();
            ushort groupId = ReadUInt16();
            IPacket packet;

            if (this.BaseStream.Position == this.BaseStream.Length)
            {
                packet = new JoinGroupPacket(cliendId, groupId);
            }
            else
            {
                string password = ReadString();
                packet = new JoinGroupPacket(cliendId, groupId, password);
            }

            return packet;
        }

		/// <summary>
		/// Used to read a JoinedGroupPacket
		/// </summary>
		/// <returns>JoinedGroupPacket</returns>
		private IPacket ReadJoinedGroupMessage()
        {
            ushort clientId = ReadUInt16();
            ushort groupId = ReadUInt16();
            IPacket packet = new JoinedGroupPacket(clientId, groupId);
            return packet;
        }

		/// <summary>
		/// Used to read a DeleteGroupPacket
		/// </summary>
		/// <returns>DeleteGroupPacket</returns>
		private IPacket ReadDeleteGroupMessage()
        {
            ushort clientId = ReadUInt16();
            ushort groupId = ReadUInt16();
            IPacket packet = new DeleteGroupPacket(clientId, groupId);
            return packet;
        }

		/// <summary>
		/// Used to read a ChangeGroupOwnerPacket
		/// </summary>
		/// <returns>ChangeGroupOwnerPacket</returns>
		private IPacket ReadChangeGroupOwnerMessage()
        {
            ushort clientId = ReadUInt16();
            ushort groupId = ReadUInt16();
            ushort newOwnerId = ReadUInt16();
            IPacket packet = new ChangeGroupOwnerPacket(clientId, groupId, newOwnerId);
            return packet;
        }

		/// <summary>
		/// Used to read a LeaveGroupPacket
		/// </summary>
		/// <returns>LeaveGroupPacket</returns>
		private IPacket ReadLeaveGroupMessage()
        {
            ushort clientId = ReadUInt16();
            ushort groupId = ReadUInt16();
            IPacket packet = new LeaveGroupPacket(clientId, groupId);
            return packet;
        }

		/// <summary>
		/// Used to read a LeftGroupPacket
		/// </summary>
		/// <returns>LeftGroupPacket</returns>
		private IPacket ReadLeftGroupMessage()
        {
            ushort clientId = ReadUInt16();
            ushort groupId = ReadUInt16();
            LeftGroupTypes type = (LeftGroupTypes)ReadByte();
            IPacket packet = new LeftGroupPacket(clientId, groupId, type);
            return packet;
        }

		/// <summary>
		/// Used to read a KickPacket
		/// </summary>
		/// <returns>KickPacket</returns>
        private IPacket ReadKickMessage()
        {
            ushort clientId = ReadUInt16();
            ushort targetId = ReadUInt16();
            ushort groupId = ReadUInt16();
            IPacket packet = new KickPacket(clientId, targetId, groupId);
            return packet;
        }

		/// <summary>
		/// Used to read a CreateGroupPacket
		/// </summary>
		/// <returns>CreateGroupPacket</returns>
		private IPacket ReadCreateGroupMessage()
        {
            ushort clientId = ReadUInt16();
            string groupName = ReadString();
            byte timeThreshold = ReadByte();
            JoinGroupPermissionTypes permissionType = (JoinGroupPermissionTypes)ReadByte();
            IPacket packet;

            if (permissionType == JoinGroupPermissionTypes.Password)
            {
                string password = ReadString();
                packet = new CreateGroupPacket(clientId, groupName, timeThreshold, permissionType, password);
            }
            else
            {
                packet = new CreateGroupPacket(clientId, groupName, timeThreshold, permissionType);
            }

            return packet;
        }

		/// <summary>
		/// Used to read a SendGroupsPacket
		/// </summary>
		/// <returns>SendGroupsPacket</returns>
		private IPacket SendGroupsMessage()
	    {
		    byte NoOfGroups = ReadByte();
		    GroupPacket[] GroupList = new GroupPacket[NoOfGroups];

		    for (int i = 0; i < NoOfGroups; i++)
		    {
			    ushort id = ReadUInt16();
			    string name = ReadString();
			    ushort ownerId = ReadUInt16();
			    JoinGroupPermissionTypes permissionType = (JoinGroupPermissionTypes)ReadByte();
			    byte timeThreshold = ReadByte();
			    GroupList[i] = new GroupPacket(name, id, ownerId, permissionType, timeThreshold);
			}
			return (IPacket) new SendGroupsPacket(GroupList);
	    }

		/// <summary>
		/// Used to read a SendUsersPacket
		/// </summary>
		/// <returns>SendUsersPacket</returns>
		private IPacket SendUsersMessage()
	    {
		    byte NoOfUsers = ReadByte();
		    CommonClientRepresentation[] UserList = new CommonClientRepresentation[NoOfUsers];

		    for (int i = 0; i < NoOfUsers; i++)
		    {
			    ushort id = ReadUInt16();
			    string name = ReadString();
			    UserList[i] = new CommonClientRepresentation() { ID = id, Name = name };
		    }
		    return (IPacket) new SendUsersPacket(UserList);
	    }

		/// <summary>
		/// Used to read a GroupPacket
		/// </summary>
		/// <returns>GroupPacket</returns>
	    private IPacket GroupPacketMessage()
	    {
		    ushort id = ReadUInt16();
		    string name = ReadString();
		    ushort ownerId = ReadUInt16();
		    JoinGroupPermissionTypes permissionType = (JoinGroupPermissionTypes) ReadByte();
		    byte timeThreshold = ReadByte();
		    byte noOfUsers = ReadByte();
			ushort[] IdList = new ushort[noOfUsers];
		    for (int i = 0; i < noOfUsers; i++)
		    {
			    ushort userId = ReadUInt16();
			    IdList[i] = userId;
		    }
		    return (IPacket) new GroupPacket(name, id, ownerId, permissionType, timeThreshold, IdList);
	    }

		/// <summary>
		/// Used to read a RequestGroupPacket
		/// </summary>
		/// <returns>RequestGroupPacket</returns>
		private IPacket RequestGroupMessage()
	    {
		    ushort groupId = ReadUInt16();
		    return (IPacket) new RequestGroupPacket(groupId);
	    }

		/// <summary>
		/// Used to read a UpdatedGroupPacket
		/// </summary>
		/// <returns>UpdatedGroupPacket</returns>
		private IPacket UpdatedGroupMessage()
	    {
		    UpdatedGroupType updateAction = (UpdatedGroupType) ReadByte();

			switch (updateAction)
			{
				case UpdatedGroupType.Created:
					GroupPacket groupPacket = (GroupPacket) GroupPacketMessage();
					return (IPacket) new UpdatedGroupPacket(updateAction, groupPacket);
				case UpdatedGroupType.Deleted:
					ushort groupId = ReadUInt16();
					return (IPacket) new UpdatedGroupPacket(updateAction, groupId);
				case UpdatedGroupType.UserJoined:
					ushort joinUserId = ReadUInt16();
					ushort joinGroupId = ReadUInt16();
					return (IPacket) new UpdatedGroupPacket(updateAction, joinUserId, joinGroupId);
				case UpdatedGroupType.UserLeft:
					ushort leftUserId = ReadUInt16();
					ushort leftGroupId = ReadUInt16();
					return (IPacket)new UpdatedGroupPacket(updateAction, leftUserId, leftGroupId);
				default:
					throw new InvalidDataException("Invalid UpdatedGroupType");
			}
		}

		/// <summary>
		/// Used to read a UpdatedUserPacket
		/// </summary>
		/// <returns>UpdatedUserPacket</returns>
		private IPacket UpdatedUserMessage()
		{
			UpdatedUserType updateAction = (UpdatedUserType)ReadByte();

			switch (updateAction)
			{
				case UpdatedUserType.Created:
					ushort clientId = ReadUInt16();
					string clientName = ReadString();
					CommonClientRepresentation client = new CommonClientRepresentation { ID = clientId, Name = clientName };
					return (IPacket) new UpdatedUserPacket(updateAction, client);
				case UpdatedUserType.Deleted:
					ushort userId = ReadUInt16();
					return (IPacket) new UpdatedUserPacket(updateAction, userId);
				default:
					throw new InvalidDataException("Invalid UpdatedUserType");
			}
		}
	}
}
