using System.IO;
using CITYMumbler.Networking.Contracts;
using CITYMumbler.Networking.Serialization;

namespace CITYMumbler.Networking.Utilities
{
    public class PacketWritter: BinaryWriter
    {
        private MemoryStream _memoryStream;

        public PacketWritter()
            : base()
        {
            this._memoryStream = new MemoryStream();
            // Set BinaryWriter's OutStream tou our memory  stream
            OutStream = _memoryStream;
        }
        

        public byte[] GetBytes()
        {
            // Close OutStream
            Close();
            // ToArray will return ONLY the bytes used, not the whole of the buffer, i.e no 0/nulls
            byte[] data = this._memoryStream.ToArray();

            return data;
        }

        public void Write(RegisterClientPacket packet)
        {
            Write(packet.Name);
        }

        public void Write(DisconnectionPacket packet)
        {
            Write(packet.ClientId);
        }

        public void Write(PrivateMessagePacket packet)
        {
            Write(packet.SenderId);
            Write(packet.ReceiverId);
            Write(packet.Message);
        }

        public void Write(SendKeystrokePacket packet)
        {
            Write((PrivateMessagePacket)packet);
        }

        public void Write(GroupMessagePacket packet)
        {
            Write(packet.SenderId);
            Write(packet.ReceiverId);
            Write(packet.Message);
        }

        public void Write(JoinGroupPacket packet)
        {
            Write(packet.CliendId);
            Write(packet.GroupId);

            if (packet.IsPasswordProtected())
                Write(packet.Password);
        }

        public void Write(JoinedGroupPacket packet)
        {
            Write(packet.ClientId);
            Write(packet.GroupId);
        }

        public void Write(DeleteGroupPacket packet)
        {
            Write(packet.ClientId);
            Write(packet.GroupId);
        }

        public void Write(ChangeGroupOwnerPacket packet)
        {
            Write(packet.ClientId);
            Write(packet.GroupId);
            Write(packet.NewOwnerId);
        }

        public void Write(LeaveGroupPacket packet)
        {
            Write(packet.ClientId);
            Write(packet.GroupId);
        }

        public void Write(LeftGroupPacket packet)
        {
            Write(packet.ClientId);
            Write(packet.GroupId);
            Write((byte)packet.LeftGroupType);
        }

        public void Write(ConnectedPacket packet)
        {
            Write(packet.ClientId);
        }

        public void Write(KickPacket packet)
        {
            Write(packet.ClientId);
            Write(packet.TargetId);
            Write(packet.GroupId);
        }

        public void Write(CreateGroupPacket packet)
        {
            Write(packet.ClientId);
            Write(packet.GroupName);
            Write(packet.TimeThreshold);
            Write((byte)packet.PermissionType);

            if (packet.PermissionType == JoinGroupPermissionTypes.Password)
                Write(packet.Password);
        }
    }
}
