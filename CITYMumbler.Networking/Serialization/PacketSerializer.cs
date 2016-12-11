using System;
using System.Text;
using CITYMumbler.Networking.Contracts;
using CITYMumbler.Networking.Contracts.Serialization;
using CITYMumbler.Networking.Utilities;

namespace CITYMumbler.Networking.Serialization
{
	/// <summary>
	/// Used to serialize/deserialize packets.
	/// </summary>
	public class PacketSerializer : IPacketSerializer
	{
		public static readonly byte[] APP_IDENTIFIER = Encoding.ASCII.GetBytes("CM");
		public static readonly byte VERSION_MAJOR = 1;
		public static readonly byte VERSION_MINOR = 0;

		/// <summary>
		/// Used to deserialize an array of bytes. Throws an ArugmentException in case the packet has the
		/// wrong app identifier, or was created with a different version of an IPacketSerializer subclass.
		/// </summary>
		/// <param name="bytes">The array of bytes received from the socket. Note that this should not include the int that specifies the length of the packet that is put at the beggining.</param>
		/// <returns>A deserialized IPacket.</returns>
		public IPacket FromBytes(byte[] bytes)
		{
			PacketReader reader = new PacketReader(bytes);
			
			// Check whether the app identifier mathes the current app
			byte identifier1 = reader.ReadByte();
			byte identifier2 = reader.ReadByte();

			if (identifier1 != APP_IDENTIFIER[0] || identifier2 != APP_IDENTIFIER[1])
				throw new ArgumentException("The APP_IDENTIFIER on this packet does not match the application");

			// check whether the serializer version matches the current version
			byte major = reader.ReadByte();
			byte minor = reader.ReadByte();

			if (major != VERSION_MAJOR || minor != VERSION_MINOR)
				throw new ArgumentException("This packet comes from a different version of the Serializer");

			// Find the type of the packet, and use the PacketReader in order to deserialze it depending on its type
			PacketType type = (PacketType)reader.ReadByte();
			IPacket packet = reader.ReadPacket(type);

			return packet;
		}

		/// <summary>
		/// Serializes a packet and returns an array of bytes that can be sent through a socket.
		/// </summary>
		/// <param name="packet">The packet to be serialized</param>
		/// <returns>The packed serialized into an array of bytes.</returns>
		public byte[] ToBytes(IPacket packet)
		{
			PacketWritter writter = new PacketWritter();

			// Start Header
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
				case PacketType.SendKeystroke:
					writter.Write((SendKeystrokePacket) packet);
					break;
				case PacketType.GroupMessage:
					writter.Write((GroupMessagePacket)packet);
					break;
				case PacketType.PrivateMessage:
					writter.Write((PrivateMessagePacket)packet);
					break;
				case PacketType.Connection:
					writter.Write((ConnectionPacket)packet);
					break;
				case PacketType.Disconnection:
					writter.Write((DisconnectionPacket) packet);
					break;
				case PacketType.Connected:
					writter.Write((ConnectedPacket)packet);
					break;
				case PacketType.CreateGroup:
					writter.Write((CreateGroupPacket)packet);
					break;
				case PacketType.DeleteGroup:
					writter.Write((DeleteGroupPacket)packet);
					break;
				case PacketType.ChangeGroupOwner:
					writter.Write((ChangeGroupOwnerPacket)packet);
					break;
				case PacketType.JoinGroup:
					writter.Write((JoinGroupPacket)packet);
					break;
				case PacketType.JoinedGroup:
					writter.Write((JoinedGroupPacket)packet);
					break;
				case PacketType.Kick:
					writter.Write((KickPacket)packet);
					break;
				case PacketType.LeaveGroup:
					writter.Write((LeaveGroupPacket)packet);
					break;
				case PacketType.LeftGroup:
					writter.Write((LeftGroupPacket)packet);
					break;
				case PacketType.SendGroups:
					writter.Write((SendGroupsPacket) packet);
					break;
				case PacketType.RequestSendGroups:
					break;
				case PacketType.SendUsers:
					writter.Write((SendUsersPacket) packet);
					break;
				case PacketType.RequestSendUsers:
					break;
				case PacketType.GroupPacket:
					writter.Write((GroupPacket) packet);
					break;
				case PacketType.RequestGroup:
					writter.Write((RequestGroupPacket) packet);
					break;
				case PacketType.UpdatedGroup:
					writter.Write((UpdatedGroupPacket)packet);
					break;
				case PacketType.UpdatedUser:
					writter.Write((UpdatedUserPacket) packet);
					break;
				default:
					throw new ArgumentException("Packet is of an unsupported packetType");
			}

			// Return the end packet
			return writter.GetBytes();
		}
	}
}
