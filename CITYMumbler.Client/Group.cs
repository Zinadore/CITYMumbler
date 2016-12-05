using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;
using ReactiveUI;

namespace CITYMumbler.Client
{
    public class Group
    {
        public ushort ID { get; set; }
        public string Name { get; set; }
        public ushort OwnerID { get; set; }
        public JoinGroupPermissionTypes PermissionType { get; set; }
        public byte TimeoutThreshold { get; set; }
        public ReactiveList<Client> GroupUsers { get; set; }
    }
}
