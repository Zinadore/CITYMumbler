﻿using System;
using System.IO;
using CITYMumbler.Common;
using CITYMumbler.Common.Data;
using CITYMumbler.Networking.Contracts;
using CITYMumbler.Networking.Contracts.Serialization;
using CITYMumbler.Networking.Serialization;

namespace CITYMumbler.Networking.Utilities
{
    public class PacketReader: BinaryReader
    {
        public PacketReader(byte[] data)
            :base(new MemoryStream(data))
        {
            
        }

        public IPacket ReadPacket(PacketType type)
        {
            switch (type)
            {
                case PacketType.Connection: return ReadRegisterClientMessage();

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

                default: throw new ArgumentException("The provided PacketType is not valid.");
            }
        }

        private IPacket ReadRegisterClientMessage()
        {
            string name = ReadString();
            IPacket packet = new ConnectionPacket(name);
            return packet;
        }

        private IPacket ReadDisconnectionMessage()
        {
            ushort clientId = ReadUInt16();
            IPacket packet = new DisconnectionPacket(clientId);
            return packet;
        }


        private IPacket ReadConnectedMessage()
        {
            ushort clientId = ReadUInt16();
            IPacket packet = new ConnectedPacket(clientId);
            return packet;
        }

        private IPacket ReadPrivateMessageMessage()
        {
            ushort senderId = ReadUInt16();
            ushort receiverId = ReadUInt16();
	        string senderName = ReadString();
            string message = ReadString();
            IPacket packet = new PrivateMessagePacket(senderId, receiverId, senderName, message);
            return packet;
        }

        private IPacket ReadKeyStrokeMessage()
        {
            ushort senderId = ReadUInt16();
            ushort receiverId = ReadUInt16();
	        string senderName = ReadString();
            string message = ReadString();
            IPacket packet = new SendKeystrokePacket(senderId, receiverId, senderName, message);
            return packet;
        }

        private IPacket ReadGroupMessageMessage()
        {
            ushort senderId = ReadUInt16();
            ushort receiverId = ReadUInt16();
	        string senderName = ReadString();
            string message = ReadString();
            IPacket packet = new GroupMessagePacket(senderId, receiverId, senderName, message);
            return packet;
        }

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

        private IPacket ReadJoinedGroupMessage()
        {
            ushort clientId = ReadUInt16();
            ushort groupId = ReadUInt16();
            IPacket packet = new JoinedGroupPacket(clientId, groupId);
            return packet;
        }

        private IPacket ReadDeleteGroupMessage()
        {
            ushort clientId = ReadUInt16();
            ushort groupId = ReadUInt16();
            IPacket packet = new DeleteGroupPacket(clientId, groupId);
            return packet;
        }

        private IPacket ReadChangeGroupOwnerMessage()
        {
            ushort clientId = ReadUInt16();
            ushort groupId = ReadUInt16();
            ushort newOwnerId = ReadUInt16();
            IPacket packet = new ChangeGroupOwnerPacket(clientId, groupId, newOwnerId);
            return packet;
        }

        private IPacket ReadLeaveGroupMessage()
        {
            ushort clientId = ReadUInt16();
            ushort groupId = ReadUInt16();
            IPacket packet = new LeaveGroupPacket(clientId, groupId);
            return packet;
        }

        private IPacket ReadLeftGroupMessage()
        {
            ushort clientId = ReadUInt16();
            ushort groupId = ReadUInt16();
            LeftGroupTypes type = (LeftGroupTypes)ReadByte();
            IPacket packet = new LeftGroupPacket(clientId, groupId, type);
            return packet;
        }

        private IPacket ReadKickMessage()
        {
            ushort clientId = ReadUInt16();
            ushort targetId = ReadUInt16();
            ushort groupId = ReadUInt16();
            IPacket packet = new KickPacket(clientId, targetId, groupId);
            return packet;
        }

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

	    private IPacket SendGroupsMessage()
	    {
		    byte NoOfGroups = ReadByte();
		    Group[] GroupList = new Group[NoOfGroups];

		    for (int i = 0; i < NoOfGroups; i++)
		    {
			    ushort id = ReadUInt16();
			    string name = ReadString();
			    ushort ownerId = ReadUInt16();
			    JoinGroupPermissionTypes permissionType = (JoinGroupPermissionTypes)ReadByte();
			    byte timeThreshold = ReadByte();
			    GroupList[i] = new Group(name, id, ownerId, permissionType, timeThreshold);
			}
			return (IPacket) new SendGroupsPacket(GroupList);
	    }

	}
}
