using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITYMumbler.Client
{
    public struct ChatEntry
    {
        public ushort SenderId { get; private set; }
        public string SenderName { get; set; }
        public string Message { get; private set; }
        public ushort? GroupId { get; private set; }

        public ChatEntry(ushort senderId, string senderName, string message, ushort? groupId = 0)
        {
            this.SenderId = senderId;
            this.SenderName = senderName;
            this.Message = message;
            this.GroupId = groupId;
        }
    }
}
