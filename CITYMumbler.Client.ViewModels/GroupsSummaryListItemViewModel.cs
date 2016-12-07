using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Client.ViewModels
{
    public class GroupsSummaryListItemViewModel: ReactiveObject, IRoutableViewModel
    {
        private Group _group;
        private MumblerClient _client;
        public ushort GroupID => _group.ID;
        public string GroupName => _group.Name;
        public string UrlPathSegment => string.Format("groupListViewItem#{0}", this._group.ID);
        public IScreen HostScreen { get; }
        public ReactiveCommand<Unit, Unit> JoinGroupCommand;

        private bool _isJoined;
        public bool IsJoined
        {
            get { return _isJoined; }
            set { this.RaiseAndSetIfChanged(ref _isJoined, value); }
        }



        public GroupsSummaryListItemViewModel(Group group, IScreen hostScreen, bool isJoined)
        {
            this._group = group;
            this._client = Locator.Current.GetService<MumblerClient>();
            this.HostScreen = hostScreen;
            this.IsJoined = isJoined;

            this.JoinGroupCommand = ReactiveCommand.Create(() => this._client.JoinGroup(this.GroupID));
        }


        
    }
}
