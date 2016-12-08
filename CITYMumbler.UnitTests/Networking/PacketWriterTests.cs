using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Data;
using CITYMumbler.Networking.Contracts;
using CITYMumbler.Networking.Contracts.Serialization;
using CITYMumbler.Networking.Serialization;
using CITYMumbler.Networking.Utilities;
using NUnit.Framework;

namespace CITYMumbler.UnitTests.Networking
{
    [TestFixture]
    class PacketReaderWriterTests
    {
        [Test]
        public void writes_string_and_packet_reader_reads_it()
        {
            // Arrange
            string data = "string";
            PacketWritter writter = new PacketWritter();

            // Act
            writter.Write(data);
            byte[] result = writter.GetBytes();

            // Assert
            PacketReader reader = new PacketReader(result);
            Assert.AreEqual(reader.ReadString(), "string");
        }

        // Register Client Packet

        [Test]
        public void register_client_packet_write()
        {
            // Arrange
            IPacket packet = new ConnectionPacket("cr@zy69");
            PacketWritter writter = new PacketWritter();

            // Act
            writter.Write((ConnectionPacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            string readResult = ((ConnectionPacket)reader.ReadPacket(PacketType.Connection)).Name;
            Assert.AreEqual(readResult, ((ConnectionPacket)packet).Name);
        }

        // Private/Group Message packet

        [Test]
        public void serializes_private_message_packet()
        {
            // Arrange
            PacketWritter writter = new PacketWritter();
            IPacket packet = new PrivateMessagePacket((ushort)2, (ushort)4, "dkdkdk", "hello world!");

            // Act
            writter.Write((PrivateMessagePacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            PrivateMessagePacket newPacket = (PrivateMessagePacket)reader.ReadPacket(PacketType.PrivateMessage);
            Assert.AreEqual(newPacket.SenderId, ((PrivateMessagePacket)packet).SenderId);
            Assert.AreEqual(newPacket.ReceiverId, ((PrivateMessagePacket)packet).ReceiverId);
            Assert.AreEqual(newPacket.Message, ((PrivateMessagePacket)packet).Message);
        }

        [Test]
        public void serializes_send_keystroke_packet()
        {
            // Arrange
            PacketWritter writter = new PacketWritter();
            IPacket packet = new SendKeystrokePacket((ushort)2, (ushort)4, "dd","h");

            // Act
            writter.Write((SendKeystrokePacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            SendKeystrokePacket newPacket = (SendKeystrokePacket)reader.ReadPacket(PacketType.SendKeystroke);
            Assert.AreEqual(newPacket.SenderId, ((SendKeystrokePacket)packet).SenderId);
            Assert.AreEqual(newPacket.ReceiverId, ((SendKeystrokePacket)packet).ReceiverId);
            Assert.AreEqual(newPacket.Message, ((SendKeystrokePacket)packet).Message);
        }

        [Test]
        public void serializes_group_message_packet()
        {
            // Arrange
            PacketWritter writter = new PacketWritter();
            IPacket packet = new GroupMessagePacket((ushort)2, (ushort)4, "dsds", "hello world!");

            // Act
            writter.Write((GroupMessagePacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            GroupMessagePacket newPacket = (GroupMessagePacket)reader.ReadPacket(PacketType.GroupMessage);
            Assert.AreEqual(newPacket.SenderId, ((GroupMessagePacket)packet).SenderId);
            Assert.AreEqual(newPacket.GroupID, ((GroupMessagePacket)packet).GroupID);
            Assert.AreEqual(newPacket.Message, ((GroupMessagePacket)packet).Message);
        }

        // Join Group Packet

        [Test]
        public void serializes_join_group_with_password_packet()
        {
            // Arrange
            PacketWritter writter = new PacketWritter();
            IPacket packet = new JoinGroupPacket((ushort)2, (ushort)4, "12345678");
            Assert.IsTrue(((JoinGroupPacket)packet).IsPasswordProtected());

            // Act
            writter.Write((JoinGroupPacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            JoinGroupPacket newPacket = (JoinGroupPacket)reader.ReadPacket(PacketType.JoinGroup);
            Assert.IsTrue(newPacket.IsPasswordProtected());
            Assert.AreEqual(newPacket.CliendId, ((JoinGroupPacket)packet).CliendId);
            Assert.AreEqual(newPacket.GroupId, ((JoinGroupPacket)packet).GroupId);
            Assert.AreEqual(newPacket.Password, ((JoinGroupPacket)packet).Password);
        }

        [Test]
        public void serializes_join_group_freely_packet()
        {
            // Arrange
            PacketWritter writter = new PacketWritter();
            IPacket packet = new JoinGroupPacket((ushort)2, (ushort)4);
            Assert.IsFalse(((JoinGroupPacket)packet).IsPasswordProtected());

            // Act
            writter.Write((JoinGroupPacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            JoinGroupPacket newPacket = (JoinGroupPacket)reader.ReadPacket(PacketType.JoinGroup);
            Assert.IsFalse(newPacket.IsPasswordProtected());
            Assert.AreEqual(newPacket.CliendId, ((JoinGroupPacket)packet).CliendId);
            Assert.AreEqual(newPacket.GroupId, ((JoinGroupPacket)packet).GroupId);
            Assert.IsNull(newPacket.Password);
        }

        // Delete Group Packet

        [Test]
        public void serializes_delete_group_packet()
        {
            // Arrange
            IPacket packet = new DeleteGroupPacket((ushort)6, (ushort)3);
            PacketWritter writter = new PacketWritter();

            // Act
            writter.Write((DeleteGroupPacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            DeleteGroupPacket newPacket = ((DeleteGroupPacket)reader.ReadPacket(PacketType.DeleteGroup));
            Assert.AreEqual(newPacket.ClientId, ((DeleteGroupPacket)packet).ClientId);
            Assert.AreEqual(newPacket.GroupId, ((DeleteGroupPacket)packet).GroupId);
        }

        // Change Group Owner Packet

        [Test]
        public void serializes_change_group_owner_packet()
        {
            // Arrange
            PacketWritter writter = new PacketWritter();
            IPacket packet = new ChangeGroupOwnerPacket((ushort)3, (ushort)2, (ushort)4);

            // Act
            writter.Write((ChangeGroupOwnerPacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            ChangeGroupOwnerPacket newPacket = (ChangeGroupOwnerPacket)reader.ReadPacket(PacketType.ChangeGroupOwner);
            Assert.AreEqual(newPacket.ClientId, ((ChangeGroupOwnerPacket)packet).ClientId);
            Assert.AreEqual(newPacket.GroupId, ((ChangeGroupOwnerPacket)packet).GroupId);
            Assert.AreEqual(newPacket.NewOwnerId, ((ChangeGroupOwnerPacket)packet).NewOwnerId);
        }

        // Leave Group Packet 

        [Test]
        public void serializes_leave_group_packet()
        {
            // Arrange
            PacketWritter writter = new PacketWritter();
            IPacket packet = new LeaveGroupPacket((ushort)3, (ushort)2);

            // Act
            writter.Write((LeaveGroupPacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            LeaveGroupPacket newPacket = (LeaveGroupPacket)reader.ReadPacket(PacketType.LeaveGroup);
            Assert.AreEqual(newPacket.ClientId, ((LeaveGroupPacket)packet).ClientId);
            Assert.AreEqual(newPacket.GroupId, ((LeaveGroupPacket)packet).GroupId);
        }

        // Left Group Packet

        [Test]
        public void serializes_left_group_kicked_packet()
        {
            // Arrange
            PacketWritter writter = new PacketWritter();
            IPacket packet = new LeftGroupPacket((ushort)3, (ushort)2, LeftGroupTypes.Kicked);

            // Act
            writter.Write((LeftGroupPacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            LeftGroupPacket newPacket = (LeftGroupPacket)reader.ReadPacket(PacketType.LeftGroup);
            Assert.AreEqual(newPacket.ClientId, ((LeftGroupPacket)packet).ClientId);
            Assert.AreEqual(newPacket.GroupId, ((LeftGroupPacket)packet).GroupId);
            Assert.AreEqual(newPacket.LeftGroupType, ((LeftGroupPacket)packet).LeftGroupType);
        }

        [Test]
        public void serializes_left_group_normal_packet()
        {
            // Arrange
            PacketWritter writter = new PacketWritter();
            IPacket packet = new LeftGroupPacket((ushort)3, (ushort)2, LeftGroupTypes.Normal);

            // Act
            writter.Write((LeftGroupPacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            LeftGroupPacket newPacket = (LeftGroupPacket)reader.ReadPacket(PacketType.LeftGroup);
            Assert.AreEqual(newPacket.ClientId, ((LeftGroupPacket)packet).ClientId);
            Assert.AreEqual(newPacket.GroupId, ((LeftGroupPacket)packet).GroupId);
            Assert.AreEqual(newPacket.LeftGroupType, ((LeftGroupPacket)packet).LeftGroupType);
        }

        // Connected

        [Test]
        public void serializes_connected_packet()
        {
            // Arrange
            PacketWritter writter = new PacketWritter();
            IPacket packet = new ConnectedPacket((ushort)3);

            // Act
            writter.Write((ConnectedPacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            ConnectedPacket newPacket = (ConnectedPacket)reader.ReadPacket(PacketType.Connected);
            Assert.AreEqual(newPacket.ClientId, ((ConnectedPacket)packet).ClientId);
        }

        // Joined Group

        [Test]
        public void serializes_joined_group_packet()
        {
            // Arrange
            PacketWritter writter = new PacketWritter();
            IPacket packet = new JoinedGroupPacket((ushort)3, (ushort)2);

            // Act
            writter.Write((JoinedGroupPacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            JoinedGroupPacket newPacket = (JoinedGroupPacket)reader.ReadPacket(PacketType.JoinedGroup);
            Assert.AreEqual(newPacket.ClientId, ((JoinedGroupPacket)packet).ClientId);
            Assert.AreEqual(newPacket.GroupId, ((JoinedGroupPacket)packet).GroupId);
        }

        // Kick Packet

        [Test]
        public void serializes_kick_packet()
        {
            // Arrange
            PacketWritter writter = new PacketWritter();
            IPacket packet = new KickPacket((ushort)3, (ushort)2, (ushort)3);

            // Act
            writter.Write((KickPacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            KickPacket newPacket = (KickPacket)reader.ReadPacket(PacketType.Kick);
            Assert.AreEqual(newPacket.ClientId, ((KickPacket)packet).ClientId);
            Assert.AreEqual(newPacket.TargetId, ((KickPacket)packet).TargetId);
            Assert.AreEqual(newPacket.GroupId, ((KickPacket)packet).GroupId);
        }

        // Create Group Packet

        [Test]
        public void serializes_create_group_free_packet()
        {
            // Arrange
            PacketWritter writter = new PacketWritter();
            IPacket packet = new CreateGroupPacket((ushort)3, "group", (byte)3, JoinGroupPermissionTypes.Free);

            // Act
            writter.Write((CreateGroupPacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            CreateGroupPacket newPacket = (CreateGroupPacket)reader.ReadPacket(PacketType.CreateGroup);
            Assert.AreEqual(newPacket.ClientId, ((CreateGroupPacket)packet).ClientId);
            Assert.AreEqual(newPacket.GroupName, ((CreateGroupPacket)packet).GroupName);
            Assert.AreEqual(newPacket.TimeThreshold, ((CreateGroupPacket)packet).TimeThreshold);
            Assert.AreEqual(newPacket.PermissionType, ((CreateGroupPacket)packet).PermissionType);
        }

        [Test]
        public void serializes_create_group_password_packet()
        {
            // Arrange
            PacketWritter writter = new PacketWritter();
            IPacket packet = new CreateGroupPacket((ushort)3, "group", (byte)3, JoinGroupPermissionTypes.Password, "password");

            // Act
            writter.Write((CreateGroupPacket)packet);
            PacketReader reader = new PacketReader(writter.GetBytes());

            // Assert
            CreateGroupPacket newPacket = (CreateGroupPacket)reader.ReadPacket(PacketType.CreateGroup);
            Assert.AreEqual(newPacket.ClientId, ((CreateGroupPacket)packet).ClientId);
            Assert.AreEqual(newPacket.GroupName, ((CreateGroupPacket)packet).GroupName);
            Assert.AreEqual(newPacket.TimeThreshold, ((CreateGroupPacket)packet).TimeThreshold);
            Assert.AreEqual(newPacket.PermissionType, ((CreateGroupPacket)packet).PermissionType);
            Assert.AreEqual(newPacket.Password, ((CreateGroupPacket)packet).Password);
        }

	    [Test]
	    public void serializes_send_groups_packet()
	    {
			// Arrange
			PacketWritter writter = new PacketWritter();
		    CommonGroupRepresentation[] groups = new CommonGroupRepresentation[]
		    {
				new CommonGroupRepresentation { Name = "group1", ID = 4,OwnerID = (byte)3, PermissionType = JoinGroupPermissionTypes.Password, TimeoutThreshold = (byte)6},
				new CommonGroupRepresentation { Name = "group2", ID = 5,OwnerID = (byte)6, PermissionType = JoinGroupPermissionTypes.Password, TimeoutThreshold = (byte)6},
				new CommonGroupRepresentation { Name = "group3", ID = 6,OwnerID = (byte)7, PermissionType = JoinGroupPermissionTypes.Password, TimeoutThreshold = (byte)6},
			};
			IPacket packet = new SendGroupsPacket(groups);

			// Act
			writter.Write((SendGroupsPacket)packet);
			PacketReader reader = new PacketReader(writter.GetBytes());

			// Assert
		    SendGroupsPacket newPacket = (SendGroupsPacket) reader.ReadPacket(PacketType.SendGroups);
		    Assert.AreEqual(newPacket.GetNoOfGroups(), groups.Length);
			Assert.AreEqual(newPacket.GroupList[0].Id, groups[0].ID);
	    }

		[Test]
		public void serializes_send_users_packet()
		{
			// Arrange
			PacketWritter writter = new PacketWritter();
			CommonClientRepresentation[] clients = new CommonClientRepresentation[]
			{
				new CommonClientRepresentation { ID = (ushort) 4, Name = "client1"},
				new CommonClientRepresentation { ID = (ushort) 5, Name = "client2"}
			};
			IPacket packet = new SendUsersPacket(clients);

			// Act
			writter.Write((SendUsersPacket)packet);
			PacketReader reader = new PacketReader(writter.GetBytes());

			// Assert
			SendUsersPacket newPacket = (SendUsersPacket)reader.ReadPacket(PacketType.SendUsers);
			Assert.AreEqual(newPacket.GetNoOfUsers(), clients.Length);
			Assert.AreEqual(newPacket.UserList[0].ID, clients[0].ID);
		}

		[Test]
		public void serializes_group_packet()
		{
			// Arrange
			PacketWritter writter = new PacketWritter();
			CommonGroupRepresentation group = new CommonGroupRepresentation {Name =  "group1", ID = (byte) 4, OwnerID = 3, PermissionType = JoinGroupPermissionTypes.Password, TimeoutThreshold = (byte) 6};
			ushort[] UserList = new ushort[2];
			UserList[0] = (ushort) 5;
			UserList[1] = (ushort) 8;
			IPacket packet = new GroupPacket(group.Name, group.ID, group.OwnerID, group.PermissionType, group.TimeoutThreshold, UserList);

			// Act
			writter.Write((GroupPacket)packet);
			PacketReader reader = new PacketReader(writter.GetBytes());

			// Assert
			GroupPacket newPacket = (GroupPacket)reader.ReadPacket(PacketType.GroupPacket);
			Assert.AreEqual(newPacket.GetNoOfUsers(), UserList.Length);
			Assert.AreEqual(newPacket.UserList[0], UserList[0]);
		}

	    [Test]
	    public void serializes_request_group_packet()
	    {
			// Arrange
			PacketWritter writter = new PacketWritter();
		    IPacket packet = new RequestGroupPacket(4);

		    // Act
			writter.Write((RequestGroupPacket) packet);
			PacketReader reader = new PacketReader(writter.GetBytes());

		    // Assert
		    RequestGroupPacket newPacket = (RequestGroupPacket) reader.ReadPacket(PacketType.RequestGroup);
			Assert.AreEqual(newPacket.GroupId, ((RequestGroupPacket) packet).GroupId);
	    }

		[Test]
		public void serializes_update_group_created_packet()
		{
			// Arrange
			PacketWritter writter = new PacketWritter();
			CommonGroupRepresentation group = new CommonGroupRepresentation() { ID = 4, Name = "group", OwnerID = 6, PermissionType = JoinGroupPermissionTypes.Free, TimeoutThreshold = 6 };
			IPacket packet = new UpdatedGroupPacket(UpdatedGroupType.Created, group);

			// Act
			writter.Write((UpdatedGroupPacket)packet);
			PacketReader reader = new PacketReader(writter.GetBytes());

			// Assert
			UpdatedGroupPacket newPacket = (UpdatedGroupPacket)reader.ReadPacket(PacketType.UpdatedGroup);
			Assert.AreEqual(newPacket.GroupPacket.Name, ((UpdatedGroupPacket)packet).GroupPacket.Name);
		}

		[Test]
		public void serializes_update_group_deleted_packet()
		{
			// Arrange
			PacketWritter writter = new PacketWritter();
			CommonGroupRepresentation group = new CommonGroupRepresentation() { ID = 4, Name = "group", OwnerID = 6, PermissionType = JoinGroupPermissionTypes.Free, TimeoutThreshold = 6 };
			IPacket packet = new UpdatedGroupPacket(UpdatedGroupType.Deleted, group.ID);

			// Act
			writter.Write((UpdatedGroupPacket)packet);
			PacketReader reader = new PacketReader(writter.GetBytes());

			// Assert
			UpdatedGroupPacket newPacket = (UpdatedGroupPacket)reader.ReadPacket(PacketType.UpdatedGroup);
			Assert.AreEqual(newPacket.GroupId, ((UpdatedGroupPacket)packet).GroupId);
		}

		[Test]
		public void serializes_update_group_user_joined_packet()
		{
			// Arrange
			PacketWritter writter = new PacketWritter();
			CommonGroupRepresentation group = new CommonGroupRepresentation() { ID = 4, Name = "group", OwnerID = 6, PermissionType = JoinGroupPermissionTypes.Free, TimeoutThreshold = 6 };
			CommonClientRepresentation user = new CommonClientRepresentation { ID = 7, Name = "user" };
			IPacket packet = new UpdatedGroupPacket(UpdatedGroupType.UserJoined, user.ID, group.ID);

			// Act
			writter.Write((UpdatedGroupPacket)packet);
			PacketReader reader = new PacketReader(writter.GetBytes());

			// Assert
			UpdatedGroupPacket newPacket = (UpdatedGroupPacket)reader.ReadPacket(PacketType.UpdatedGroup);
			Assert.AreEqual(((UpdatedGroupPacket)packet).UserId, newPacket.UserId);
			Assert.AreEqual(((UpdatedGroupPacket)packet).GroupId, newPacket.GroupId);
		}

		[Test]
		public void serializes_update_group_user_left_packet()
		{
			// Arrange
			PacketWritter writter = new PacketWritter();
			CommonGroupRepresentation group = new CommonGroupRepresentation() { ID = 4, Name = "group", OwnerID = 6, PermissionType = JoinGroupPermissionTypes.Free, TimeoutThreshold = 6 };
			CommonClientRepresentation user = new CommonClientRepresentation { ID = 7, Name = "user" };
			IPacket packet = new UpdatedGroupPacket(UpdatedGroupType.UserLeft, user.ID, group.ID);

			// Act
			writter.Write((UpdatedGroupPacket)packet);
			PacketReader reader = new PacketReader(writter.GetBytes());

			// Assert
			UpdatedGroupPacket newPacket = (UpdatedGroupPacket)reader.ReadPacket(PacketType.UpdatedGroup);
			Assert.AreEqual(((UpdatedGroupPacket)packet).UserId, newPacket.UserId);
			Assert.AreEqual(((UpdatedGroupPacket)packet).GroupId, newPacket.GroupId);
		}

		[Test]
		public void serializes_update_user_created_packet()
		{
			// Arrange
			PacketWritter writter = new PacketWritter();
			CommonClientRepresentation user = new CommonClientRepresentation { ID = 7, Name = "user" };
			IPacket packet = new UpdatedUserPacket(UpdatedUserType.Created, user);

			// Act
			writter.Write((UpdatedUserPacket)packet);
			PacketReader reader = new PacketReader(writter.GetBytes());

			// Assert
			UpdatedUserPacket newPacket = (UpdatedUserPacket)reader.ReadPacket(PacketType.UpdatedUser);
			Assert.AreEqual(((UpdatedUserPacket)packet).Client.Name, newPacket.Client.Name);
		}

		[Test]
		public void serializes_update_user_deleted_packet()
		{
			// Arrange
			PacketWritter writter = new PacketWritter();
			CommonClientRepresentation user = new CommonClientRepresentation { ID = 7, Name = "user" };
			IPacket packet = new UpdatedUserPacket(UpdatedUserType.Deleted, user.ID);

			// Act
			writter.Write((UpdatedUserPacket)packet);
			PacketReader reader = new PacketReader(writter.GetBytes());

			// Assert
			UpdatedUserPacket newPacket = (UpdatedUserPacket)reader.ReadPacket(PacketType.UpdatedUser);
			Assert.AreEqual(((UpdatedUserPacket)packet).UserId, newPacket.UserId);
		}
	}
}
