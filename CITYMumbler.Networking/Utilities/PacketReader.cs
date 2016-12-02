using System;
using System.IO;
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

        public IPacket ReadPacket(PacketTypeHeader type)
        {
            switch (type)
            {
                case PacketTypeHeader.Connection: return ReadRegisterClientMessage();

                case PacketTypeHeader.Disconnection: return ReadDisconnectionMessage();

                case PacketTypeHeader.Connected: return ReadConnectedMessage();

                case PacketTypeHeader.PrivateMessage: return ReadPrivateMessageMessage();

                case PacketTypeHeader.SendKeystroke: return ReadKeyStrokeMessage();

                case PacketTypeHeader.GroupMessage: return ReadGroupMessageMessage();

                case PacketTypeHeader.JoinGroup: return ReadJoinGroupMessage();

                case PacketTypeHeader.JoinedGroup: return ReadJoinedGroupMessage();

                case PacketTypeHeader.DeleteGroup: return ReadDeleteGroupMessage();

                case PacketTypeHeader.ChangeGroupOwner: return ReadChangeGroupOwnerMessage();

                case PacketTypeHeader.LeaveGroup: return ReadLeaveGroupMessage();

                case PacketTypeHeader.LeftGroup: return ReadLeftGroupMessage();

                case PacketTypeHeader.Kick: return ReadKickMessage();

                case PacketTypeHeader.CreateGroup: return ReadCreateGroupMessage();

                default: throw new ArgumentException("The provided PacketTypeHeader is not valid.");
            }
        }

        private IPacket ReadRegisterClientMessage()
        {
            string name = ReadString();
            IPacket packet = new RegisterClientPacket(name);
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
    }
}
