using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CITYMumbler.Client
{
    public class PrivateChat
    {
        public Client LocalUser { get; private set; }
        public Client RemoteUser { get; private set; }

        public PrivateChat(Client local, Client remote)
        {
            this.LocalUser = local;
            this.RemoteUser = remote;
        }
    }
}
