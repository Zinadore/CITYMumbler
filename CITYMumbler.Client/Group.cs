using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;
using ReactiveUI;

namespace CITYMumbler.Client
{
    /// <summary>
    /// An object representation of a group, used on the client-side.
    /// </summary>
    public class Group: ReactiveObject
    {
        private ushort _iD;
        public ushort ID
        {
            get { return _iD; }
            set { this.RaiseAndSetIfChanged(ref _iD, value); }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
        private ushort _ownerID;
        public ushort OwnerID
        {
            get { return _ownerID; }
            set { this.RaiseAndSetIfChanged(ref _ownerID, value); }
        }
        private JoinGroupPermissionTypes _permissionType;
        public JoinGroupPermissionTypes PermissionType
        {
            get { return _permissionType; }
            set { this.RaiseAndSetIfChanged(ref _permissionType, value); }
        }
        private byte _timeoutThreshold;
        public byte TimeoutThreshold
        {
            get { return _timeoutThreshold; }
            set { this.RaiseAndSetIfChanged(ref _timeoutThreshold, value); }
        }
        public ReactiveList<Client> GroupUsers { get; set; }

        public override bool Equals(object obj)
        {
            Group other = obj as Group;
            if (other == null)
                return false;
            return this.ID == other.ID;
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }
    }
}
