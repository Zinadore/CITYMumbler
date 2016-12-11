using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITYMumbler.Client
{
    /// <summary>
    /// A chat entry. Can be used for either a group or a private message. Note it is a struct, NOT a class.
    /// </summary>
    public struct ChatEntry
    {
        public ushort SenderId { get; private set; }
        public ushort ReceiverId { get; private set; }
        public string SenderName { get; set; }
        public string Message { get; private set; }
        public ushort? GroupId { get; private set; }

        /// <summary>
        /// The only construtor. groupid should be passed only if the entry is for a group message. Similarly the receiverId should be
        /// passed only for a private chat message
        /// </summary>
        /// <param name="senderId">The id of the client that sent the message</param>
        /// <param name="senderName">The name of the client that sent the message.</param>
        /// <param name="message">The message of the entry</param>
        /// <param name="groupId">The id of the target group. Used only for group messages</param>
        /// <param name="receiverId">The id of the receiver. Used only for private messages</param>
        public ChatEntry(ushort senderId, string senderName, string message, ushort groupId = 0, ushort receiverId = 0)
        {
            this.SenderId = senderId;
            this.SenderName = senderName;
            this.Message = message;
            this.GroupId = groupId;
            this.ReceiverId = receiverId;
        }
    }
}
