using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Data;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Client.ViewModels
{
    public class SummaryViewModel: ReactiveObject, ISupportsActivation
    {
        #region Private Members
        private readonly MumblerClient _client;
        private readonly ViewModelActivator _viewModelActivator = new ViewModelActivator();
        #endregion

        private ReactiveList<UsersSummaryListItemViewModel> _users;
        public ReactiveList<UsersSummaryListItemViewModel> Users
        {
            get { return _users; }
            set { this.RaiseAndSetIfChanged(ref _users, value); }
        }


        private ReactiveList<GroupsSummaryListItemViewModel> _name;
        public ReactiveList<GroupsSummaryListItemViewModel> Groups
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
        
        public SummaryViewModel()
        {
            this._client = Locator.Current.GetService<MumblerClient>();
            this.Groups = new ReactiveList<GroupsSummaryListItemViewModel>();
            this.Users = new ReactiveList<UsersSummaryListItemViewModel>();

            this.WhenActivated(d =>
            {
                foreach (var group in this._client.Groups)
                {
                    addNewGroup(group);
                }

                foreach (var user in this._client.ConnectedUsers)
                {
                    addNewUser(user);
                }

                d(this._client.Groups.ItemsAdded.Subscribe(addNewGroup));

                d(this._client.Groups.ItemsRemoved.Subscribe(addNewGroup));

                d(this._client.JoinedGroups.ItemsAdded.ObserveOn(RxApp.MainThreadScheduler).Subscribe(group =>
                {
                    var groupToChange = this.Groups.FirstOrDefault(g => g.GroupID == group.ID);
                    if (groupToChange!= null)
                        groupToChange.IsJoined = true;
                    this.orderGroups();
                }));

                d(this._client.JoinedGroups.ItemsRemoved.ObserveOn(RxApp.MainThreadScheduler).Subscribe(group =>
                {
                    var groupToChange = this.Groups.FirstOrDefault(g => g.GroupID == group.ID);
                    if (groupToChange != null)
                        groupToChange.IsJoined = false;
                    this.orderGroups();
                }));

                d(this._client.ConnectedUsers.ItemsAdded.ObserveOn(RxApp.MainThreadScheduler).Subscribe(addNewUser));

                d(this._client.ConnectedUsers.ItemsRemoved.ObserveOn(RxApp.MainThreadScheduler).Subscribe(user =>
                {
                    var v = this.Users.First(vm => vm.ID == user.ID);
                    v.OnStartWhisper -= Vm_OnStartWhisper;
                    this.Users.Remove(v);
                }));
            });
            
        }

        

        private void addNewGroup(Group group)
        {
            var g = group.GroupUsers.FirstOrDefault(c => c.ID == this._client.ID);
            bool isJoined = g != null;
            this.Groups.Add(new GroupsSummaryListItemViewModel(group, Locator.Current.GetService<IScreen>(), isJoined));
        }

        private void addNewUser(Client client)
        {
            var vm = new UsersSummaryListItemViewModel(client);
            vm.OnStartWhisper += Vm_OnStartWhisper;
            this.Users.Add(vm);
        }

        private void Vm_OnStartWhisper(object sender, WhisperEventArgs e)
        {
            this._client.Whisper(e.WhisperID);
        }

        private void orderGroups()
        {
            var list  = this.Groups.OrderByDescending(g => g.IsJoined).ThenBy(g => g.GroupID).ToList();
            this.Groups = new ReactiveList<GroupsSummaryListItemViewModel>(list);
        }

        public ViewModelActivator Activator
        {
            get { return this._viewModelActivator; }
        }
    }
}
