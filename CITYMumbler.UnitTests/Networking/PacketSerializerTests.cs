using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Data;
using CITYMumbler.Networking.Contracts;
using CITYMumbler.Networking.Contracts.Serialization;
using CITYMumbler.Networking.Serialization;
using NUnit.Framework;

namespace CITYMumbler.UnitTests.Networking
{
    [TestFixture]
    class PacketSerializerTests
    {
        [Test]
        public void serialize_packet_to_bytes()
        {
            // Arrange
            IPacket packet = new PrivateMessagePacket((ushort)3, (ushort)6, "ddd", "hello");
            PacketSerializer serializer = new PacketSerializer();

            // Act
            byte[] result = serializer.ToBytes(packet);

            // Assert
            BinaryReader reader = new BinaryReader(new MemoryStream(result));
            Assert.AreEqual(Encoding.ASCII.GetString(reader.ReadBytes(2)), Encoding.ASCII.GetString(PacketSerializer.APP_IDENTIFIER));
            Assert.AreEqual(reader.ReadByte(), PacketSerializer.VERSION_MAJOR);
            Assert.AreEqual(reader.ReadByte(), PacketSerializer.VERSION_MINOR);
            Assert.AreEqual(reader.ReadByte(), (byte)packet.PacketType);
        }

        [Test]
        public void deserialize_packet_from_bytes()
        {
            // Arrange
            IPacket packet = new PrivateMessagePacket((ushort)3, (ushort)6, "Ddd","hello");
            PacketSerializer serializer = new PacketSerializer();

            // Act
            byte[] bytes = serializer.ToBytes(packet);
            IPacket result = serializer.FromBytes(bytes);

            // Assert
            Assert.AreEqual(result.PacketType, packet.PacketType);
            PrivateMessagePacket castResult = (PrivateMessagePacket)result;
            PrivateMessagePacket castPacket = (PrivateMessagePacket)packet;
            Assert.AreEqual(castResult.SenderId, castPacket.SenderId);
            Assert.AreEqual(castResult.ReceiverId, castPacket.ReceiverId);
            Assert.AreEqual(castResult.Message, castPacket.Message);
        }

        [Test]
        public void deserialize_throws_exception_if_app_identifier_wrong()
        {
            // Arrange 
            IPacket packet = new PrivateMessagePacket((ushort)3, (ushort)6, "elel","hello");
            PacketSerializer serializer = new PacketSerializer();

            // Act
            byte[] bytes = serializer.ToBytes(packet);
            bytes[0] = Convert.ToByte('B');

            // Assert
            var ex = Assert.Throws<ArgumentException>(() => serializer.FromBytes(bytes));
            Assert.That(ex.Message, Is.EqualTo("The APP_IDENTIFIER on this packet does not match the application"));
        }

        [Test]
        public void deserialize_throws_exception_if_version_is_wrong()
        {
            // Arrange 
            IPacket packet = new PrivateMessagePacket((ushort)3, (ushort)6, "edd", "hello");
            PacketSerializer serializer = new PacketSerializer();

            // Act
            byte[] bytes = serializer.ToBytes(packet);
            bytes[3] = 5;

            // Assert
            var ex = Assert.Throws<ArgumentException>(() => serializer.FromBytes(bytes));
            Assert.That(ex.Message, Is.EqualTo("This packet comes from a different version of the Serializer"));
        }


        [Test]
        public void deserialize_throws_exception_if_packetType_does_not_exist()
        {
            // Arrange 
            IPacket packet = new PrivateMessagePacket((ushort)3, (ushort)6, "ddks", "hello");
            PacketSerializer serializer = new PacketSerializer();

            // Act
            byte[] bytes = serializer.ToBytes(packet);
            bytes[4] = 104;

            // Assert
            var ex = Assert.Throws<ArgumentException>(() => serializer.FromBytes(bytes));
            Assert.That(ex.Message, Is.EqualTo("The provided PacketType is not valid."));
        }

	    [Test]
	    public void deserialize_send_goups()
	    {
			// Arrange
			Group[] groups = new Group[]
			{
				new Group("group1", (byte) 1,(byte)1, JoinGroupPermissionTypes.Password, (byte)6),
				new Group("group2", (byte) 2,(byte)2, JoinGroupPermissionTypes.Password, (byte)6),
				new Group("group3", (byte) 3,(byte)3, JoinGroupPermissionTypes.Password, (byte)6)
			};
			IPacket packet = new SendGroupsPacket(groups);
			PacketSerializer serializer = new PacketSerializer();

			// Act
		    byte[] bytes = serializer.ToBytes(packet);
		    SendGroupsPacket result = (SendGroupsPacket) serializer.FromBytes(bytes);

		    // Assert
			Assert.AreEqual(result.GetNoOfGroups(), groups.Length);
			Assert.AreEqual(result.GroupList[0].Id, groups[0].Id);
	    }

		[Test]
		public void deserializerequest_groups_list()
		{
			// Arrange 
			IPacket packet = new RequestSendGroupsPacket();
			PacketSerializer serializer = new PacketSerializer();

			// Act
			byte[] bytes = serializer.ToBytes(packet);

			// Assert
			RequestSendGroupsPacket newPacket = (RequestSendGroupsPacket) serializer.FromBytes(bytes);
			Assert.AreEqual(packet.PacketType, newPacket.PacketType);
		}

		[Test]
		public void deserialize_send_users()
		{
			// Arrange
			Common.Data.Client[] users = new Common.Data.Client[]
			{
				new Common.Data.Client((ushort) 4, "name1"),
				new Common.Data.Client((ushort) 6, "name2"),
				new Common.Data.Client((ushort) 7, "name3")
			};
			IPacket packet = new SendUsersPacket(users);
			PacketSerializer serializer = new PacketSerializer();

			// Act
			byte[] bytes = serializer.ToBytes(packet);
			SendUsersPacket result = (SendUsersPacket)serializer.FromBytes(bytes);

			// Assert
			Assert.AreEqual(result.GetNoOfUsers(), users.Length);
			Assert.AreEqual(result.UserList[0].Name, users[0].Name);
		}

		[Test]
		public void deserialize_request_users_list()
		{
			// Arrange 
			IPacket packet = new RequestSendUsersPacket();
			PacketSerializer serializer = new PacketSerializer();

			// Act
			byte[] bytes = serializer.ToBytes(packet);

			// Assert
			RequestSendUsersPacket newPacket = (RequestSendUsersPacket)serializer.FromBytes(bytes);
			Assert.AreEqual(packet.PacketType, newPacket.PacketType);
		}

	    [Test]
	    public void deserializer_group_packet()
	    {
			// Arrange 
			Group group = new Group("group1", (byte)4, (byte)3, JoinGroupPermissionTypes.Password, (byte)6);
			ushort[] UserList = new ushort[2];
			UserList[0] = (ushort)5;
			UserList[1] = (ushort)8;
			IPacket packet = new GroupPacket(group.Name, group.Id, group.ownerId, group.PermissionType, group.TimeThreshold, UserList);
			PacketSerializer serializer = new PacketSerializer();

			// Act
			byte[] bytes = serializer.ToBytes(packet);

			// Assert
		    GroupPacket newPacket = (GroupPacket) serializer.FromBytes(bytes);
			Assert.AreEqual(((GroupPacket)packet).Name, newPacket.Name);
			Assert.AreEqual(UserList.Length, newPacket.GetNoOfUsers());
			Assert.AreEqual(UserList[0], newPacket.UserList[0]);
	    }
	}
}
