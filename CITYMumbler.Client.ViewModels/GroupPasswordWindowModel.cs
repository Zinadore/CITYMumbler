using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace CITYMumbler.Client.ViewModels
{
    public class GroupPasswordWindowModel: ReactiveObject
    {
        private readonly Group _group;
        public string Name => this._group.Name;

        private string _password;
        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        public GroupPasswordWindowModel(Group group) { this._group = group; }
    }
}
