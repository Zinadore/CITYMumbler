using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Contracts.Services.Logger;
using CITYMumbler.Networking.Contracts;
using ReactiveUI;
using Splat;
using ILogger = CITYMumbler.Common.Contracts.Services.Logger.ILogger;
using LogLevel = CITYMumbler.Common.Contracts.Services.Logger.LogLevel;


namespace CITYMumbler.Client.ViewModels
{
	public class MainViewModel : ReactiveObject, IRoutableViewModel
	{
	    private MumblerClient _client;
	    private ILogger _logger;
	    private ReactiveList<Client> _dummyList;
	    private Interaction<CreateGroupWindowModel, bool> _createGroupInteraction;
		public string UrlPathSegment => "mainView";
		public IScreen HostScreen { get; }
	    private ChatViewModel _selectedTab;
	    public ChatViewModel SelectedTab
	    {
	        get { return _selectedTab; }
	        set { this.RaiseAndSetIfChanged(ref _selectedTab, value); }
	    }

	    private readonly ObservableAsPropertyHelper<ReactiveList<Client>> _currentUsers;
	    public ReactiveList<Client> CurrentUsers
	    {
	        get { return _currentUsers.Value; }
	    }


		public ReactiveList<ChatViewModel> ChatList { get; private set; }
        public ReactiveList<CurrentChatUserListItemViewModel> CurrentUsersVMs { get; private set; }
	    public Interaction<CreateGroupWindowModel, bool> CreateGroupInteraction => this._createGroupInteraction;
        public ReactiveCommand<Unit, Unit> CreateGroupCommand { get; private set; }
        
	    public string MyUsername => this._client.Name;


	    public MainViewModel(IScreen hostScreen)
	    {
	        this.HostScreen = hostScreen; 
	        this.ChatList = new ReactiveList<ChatViewModel>();
	        this._client = Locator.Current.GetService<MumblerClient>();
	        this._logger = Locator.Current.GetService<ILoggerService>().GetLogger(this.GetType());
            this._dummyList = new ReactiveList<Client>();
            this.CurrentUsersVMs = new ReactiveList<CurrentChatUserListItemViewModel>();
            this._createGroupInteraction = new Interaction<CreateGroupWindowModel, bool>();
            
	        this.CreateGroupCommand = ReactiveCommand.CreateFromTask(CreateGroup);

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
	            var vm = this.ChatList.FirstOrDefault(c => c.ChatType == ChatViewModelType.GroupChat && c.RemoteID == group.ID);
	            this.ChatList.Remove(vm);
	            this.SelectedTab = this.ChatList.FirstOrDefault();
	        });

	        this._client.PrivateChats.ItemsAdded.ObserveOn(RxApp.MainThreadScheduler).Subscribe(chat =>
	        {
                var vm = new ChatViewModel(this.HostScreen, ChatViewModelType.PrivateChat, chat);
                this.ChatList.Add(vm);
	            this.SelectedTab = vm;
	        });

	        this._client.PrivateChats.ItemsRemoved.ObserveOn(RxApp.MainThreadScheduler).Subscribe(chat =>
	        {
	            var v = this.ChatList.FirstOrDefault(vm => vm.ChatType == ChatViewModelType.PrivateChat && vm.RemoteID == chat.RemoteUser.ID);
	            if (v != null)
	            {
	                this.ChatList.Remove(v);
	                this.SelectedTab = this.ChatList.FirstOrDefault();
	            }
	        });

            var chatVmObservable = this.WhenAnyValue(x => x.SelectedTab);


	        chatVmObservable
	            .Select(cvm =>
	            {
	                if (cvm == null || cvm.ChatType == ChatViewModelType.PrivateChat)
	                    return new ReactiveList<Client>();
	                else
	                    return cvm.Group.GroupUsers;
	            })
	            .ToProperty(this, x => x.CurrentUsers, out this._currentUsers);

	        this.WhenAnyValue(x => x.CurrentUsers)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Where(list => list != null)
	            .Subscribe(list =>
	            {
                    this.CurrentUsersVMs.Clear();
	                foreach (var client in list)
	                {
	                    this.CurrentUsersVMs.Add(new CurrentChatUserListItemViewModel(client, this._client, this.SelectedTab.Group));
	                }

	                list.ItemsAdded.ObserveOn(RxApp.MainThreadScheduler).Subscribe(c =>
	                {
                        this.CurrentUsersVMs.Add(new CurrentChatUserListItemViewModel(c, this._client, this.SelectedTab.Group));
                    });

	                list.ItemsRemoved.ObserveOn(RxApp.MainThreadScheduler).Subscribe(c =>
	                {
	                    this.CurrentUsersVMs.Remove(this.CurrentUsersVMs.FirstOrDefault(vm => vm.ID == c.ID));
	                });

	            });


	    }

	    private async Task CreateGroup()
	    {
	        var vm = new CreateGroupWindowModel();
	        var wasConfirmed = await this._createGroupInteraction.Handle(vm);
	        if (wasConfirmed)
	        {
	            if (vm.SelectedAuthentication.Type == JoinGroupPermissionTypes.Free)
	            {
	                this._client.CreateGroup(vm.Name, vm.SelectedAuthentication.Type, vm.ThresholdNumeric);
	            }
	            else
	            {
                    this._client.CreateGroup(vm.Name, vm.SelectedAuthentication.Type, vm.ThresholdNumeric, vm.Password);
                }
	        }
	    }
    }
}
