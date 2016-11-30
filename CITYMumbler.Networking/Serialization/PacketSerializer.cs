using System;
using System.Text;
using CITYMumbler.Networking.Contracts;
using CITYMumbler.Networking.Contracts.Serialization;
using CITYMumbler.Networking.Utilities;

namespace CITYMumbler.Networking.Serialization
{
	public class PacketSerializer : IPacketSerializer
	{
		public static readonly byte[] APP_IDENTIFIER = Encoding.ASCII.GetBytes("CM");
		public static readonly byte VERSION_MAJOR = 1;
		public static readonly byte VERSION_MINOR = 0;

		public IPacket FromBytes(byte[] bytes)
		{
			PacketReader reader = new PacketReader(bytes);

			byte identifier1 = reader.ReadByte();
			byte identifier2 = reader.ReadByte();

			if (identifier1 != APP_IDENTIFIER[0] || identifier2 != APP_IDENTIFIER[1])
				throw new ArgumentException("The APP_IDENTIFIER on this packet does not match the application");

			byte major = reader.ReadByte();
			byte minor = reader.ReadByte();

			if (major != VERSION_MAJOR || minor != VERSION_MINOR)
				throw new ArgumentException("This packet comes from a different version of the Serializer");

			PacketTypeHeader type = (PacketTypeHeader)reader.ReadByte();
			IPacket packet = reader.ReadPacket(type);

			return packet;
		}

		public byte[] ToBytes(IPacket packet)
		{
			PacketWritter writter = new PacketWritter();

			// Write Header
			//
			// Write the App identifier and Version
			writter.Write(APP_IDENTIFIER);
			writter.Write(VERSION_MAJOR);
			writter.Write(VERSION_MINOR);

			// Write the packetType
			writter.Write((byte) packet.PacketType);
			// 
			// End Header

			// Write Payload
			switch (packet.PacketType)
			{
				case PacketTypeHeader.SendKeystroke:
					writter.Write((SendKeystrokePacket) packet);
					break;
				case PacketTypeHeader.GroupMessage:
					writter.Write((GroupMessagePacket)packet);
					break;
				case PacketTypeHeader.PrivateMessage:
					writter.Write((PrivateMessagePacket)packet);
					break;
				case PacketTypeHeader.Connection:
					writter.Write((RegisterClientPacket)packet);
					break;
				case PacketTypeHeader.Disconnection:
					writter.Write((DisconnectionPacket) packet);
					break;
				case PacketTypeHeader.Connected:
					writter.Write((ConnectedPacket)packet);
					break;
				case PacketTypeHeader.CreateGroup:
					writter.Write((CreateGroupPacket)packet);
					break;
				case PacketTypeHeader.DeleteGroup:
					writter.Write((DeleteGroupPacket)packet);
					break;
				case PacketTypeHeader.ChangeGroupOwner:
					writter.Write((ChangeGroupOwnerPacket)packet);
					break;
				case PacketTypeHeader.JoinGroup:
					writter.Write((JoinGroupPacket)packet);
					break;
				case PacketTypeHeader.JoinedGroup:
					writter.Write((JoinedGroupPacket)packet);
					break;
				case PacketTypeHeader.Kick:
					writter.Write((KickPacket)packet);
					break;
				case PacketTypeHeader.LeaveGroup:
					writter.Write((LeaveGroupPacket)packet);
					break;
				case PacketTypeHeader.LeftGroup:
					writter.Write((LeftGroupPacket)packet);
					break;
				default:
					throw new ArgumentException("Packet is of an unsupported packetType");
			}

			// Return the end packet
			return writter.GetBytes();
		}
	}
}
