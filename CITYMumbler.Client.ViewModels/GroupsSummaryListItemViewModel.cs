using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Client.ViewModels
{
    public class GroupsSummaryListItemViewModel: ReactiveObject, IRoutableViewModel
    {
        private Group _group;
        private MumblerClient _client;
        private readonly Interaction<GroupPasswordWindowModel, bool> groupPasswordInteraction;
        public ushort GroupID => _group.ID;
        public string GroupName => _group.Name;
        public string UrlPathSegment => string.Format("groupListViewItem#{0}", this._group.ID);
        public IScreen HostScreen { get; }
        public ReactiveCommand<Unit, Unit> JoinGroupCommand;
        public ReactiveCommand<Unit, Unit> LeaveGroupCommand;
        public Interaction<GroupPasswordWindowModel, bool> GroupPasswordInteraction => groupPasswordInteraction;

        private bool _isJoined;
        public bool IsJoined
        {
            get { return _isJoined; }
            set { this.RaiseAndSetIfChanged(ref _isJoined, value); }
        }



        public GroupsSummaryListItemViewModel(Group group, IScreen hostScreen, bool isJoined)
        {
            this._group = group;
            {
                this._client = Locator.Current.GetService<MumblerClient>();
                this.HostScreen = hostScreen;
                this.IsJoined = isJoined;
                this.groupPasswordInteraction = new Interaction<GroupPasswordWindowModel, bool>();

                this.JoinGroupCommand = ReactiveCommand.CreateFromTask(JoinGroup);
                this.LeaveGroupCommand = ReactiveCommand.Create(() =>
                {
                    this._client.LeaveGroup(this.GroupID);
                    this.IsJoined = false;
                });
            }

        }

        private async Task JoinGroup()
        {
            if (this._group.PermissionType == JoinGroupPermissionTypes.Password)
            {
                var vm = new GroupPasswordWindowModel(this._group);
                var wasConfirmed = await this.groupPasswordInteraction.Handle(vm);
                if (wasConfirmed && !string.IsNullOrWhiteSpace(vm.Password))
                {
                    this._client.JoinGroup(this.GroupID, vm.Password);
                }
            }
            else
            {
                this._client.JoinGroup(this.GroupID);
            }
        }

    }
}
