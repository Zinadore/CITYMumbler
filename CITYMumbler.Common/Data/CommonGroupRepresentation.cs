using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;

namespace CITYMumbler.Common.Data
{
    public class CommonGroupRepresentation
    {
        public ushort ID { get; set; }
        public string Name { get; set; }
        public ushort OwnerID { get; set; }
        public JoinGroupPermissionTypes PermissionType { get; set; }
        public byte TimeoutThreshold { get; set; }
        public ushort[] UserIdList { get; set; }
    }
}
