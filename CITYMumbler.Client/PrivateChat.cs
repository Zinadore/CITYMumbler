using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITYMumbler.Client
{
    /// <summary>
    /// A class that represents a private conversation with a client.
    /// </summary>
    public class PrivateChat
    {
        /// <summary>
        /// The local client (HINT: Us in the conversation)
        /// </summary>
        public Client LocalUser { get; private set; }

        /// <summary>
        /// The remote client (HINT: Them in the conversation)
        /// </summary>
        public Client RemoteUser { get; private set; }

        public PrivateChat(Client local, Client remote)
        {
            this.LocalUser = local;
            this.RemoteUser = remote;
        }
    }
}
