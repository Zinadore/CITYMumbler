using CITYMumbler.Networking.Contracts;
using ReactiveUI;

namespace CITYMumbler.Server
{
    internal class Group
    {
        public ushort ID { get; set; }
        public string Name { get; set; }
        public ushort OwnerID { get; set; }
        public JoinGroupPermissionTypes PermissionType { get; set; }
        public byte Threshold { get; set; }
        public string Password { get; set; }
        public ReactiveList<Client> Clients { get; set; }

    }
}
