using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Client.ViewModels
{
	public class MainViewModel : ReactiveObject, IRoutableViewModel
	{
	    private MumblerClient _client;
		public string UrlPathSegment => "mainView";
		public IScreen HostScreen { get; }
	    private ChatViewModel _selectedTab;
	    public ChatViewModel SelectedTab
	    {
	        get { return _selectedTab; }
	        set { this.RaiseAndSetIfChanged(ref _selectedTab, value); }
	    }

	    private ReactiveList<Client> _currentUsers;
	    public ReactiveList<Client> CurrentUsers
	    {
	        get { return _currentUsers; }
	        set { this.RaiseAndSetIfChanged(ref _currentUsers, value); }
	    }



		public ReactiveList<ChatViewModel> ChatList { get; private set; }
        public ReactiveList<CurrentChatUserListItemViewModel> CurrentUsersVMs { get; private set; }
	    public MainViewModel(IScreen hostScreen)
	    {
	        this.HostScreen = hostScreen; 
	        this.ChatList = new ReactiveList<ChatViewModel>();
	        this._client = Locator.Current.GetService<MumblerClient>();
            this.CurrentUsersVMs = new ReactiveList<CurrentChatUserListItemViewModel>();

	        foreach (Group g in this._client.JoinedGroups)
	        {
                this.ChatList.Add(new ChatViewModel(this.HostScreen, ChatViewModelType.GroupChat, g));
            }

	        foreach (PrivateChat pc in this._client.PrivateChats)
	        {
	            this.ChatList.Add(new ChatViewModel(this.HostScreen, ChatViewModelType.PrivateChat, pc));
	        }

	        this._client.JoinedGroups.ItemsAdded.ObserveOn(RxApp.MainThreadScheduler).Subscribe(group =>
	        {
	            var vm = new ChatViewModel(this.HostScreen, ChatViewModelType.GroupChat, group);
                this.ChatList.Add(vm);
	            this.SelectedTab = vm;
	        });

	        this._client.JoinedGroups.ItemsRemoved.ObserveOn(RxApp.MainThreadScheduler).Subscribe(group =>
	        {
	            var vm = this.ChatList.FirstOrDefault(c => c.RemoteID == group.ID);
	            this.ChatList.Remove(vm);
	            this.SelectedTab = this.ChatList.FirstOrDefault();
	        });

	        this._client.PrivateChats.ItemsAdded.ObserveOn(RxApp.MainThreadScheduler).Subscribe(chat =>
	        {
                var vm = new ChatViewModel(this.HostScreen, ChatViewModelType.PrivateChat, chat);
                this.ChatList.Add(vm);
	            this.SelectedTab = vm;
	        });

	        this.WhenAnyValue(x => x.SelectedTab)
                .Where(tab => tab != null)
	            .Where(tab => tab.ChatType == ChatViewModelType.GroupChat)
	            .Select(cvm => cvm.Group.GroupUsers)
	            .ToProperty(this, x => x.CurrentUsers);

	        this.WhenAnyValue(x => x.CurrentUsers)
                .Where(list => list != null)
	            .Subscribe(list =>
	            {
                    this.CurrentUsersVMs.Clear();
	                foreach (var client in list)
	                {
	                    this.CurrentUsersVMs.Add(new CurrentChatUserListItemViewModel(client, this.SelectedTab.Group.OwnerID));
	                }

	                list.ItemsAdded.ObserveOn(RxApp.MainThreadScheduler).Subscribe(c =>
	                {
                        this.CurrentUsersVMs.Add(new CurrentChatUserListItemViewModel(c, this.SelectedTab.Group.OwnerID));
                    });

	                list.ItemsRemoved.ObserveOn(RxApp.MainThreadScheduler).Subscribe(c =>
	                {
	                    this.CurrentUsersVMs.Remove(this.CurrentUsersVMs.FirstOrDefault(vm => vm.ID == c.ID));
	                });

	            });


	    }

    }
}
