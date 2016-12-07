using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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


		public ReactiveList<ChatViewModel> ChatList { get; private set; }

	    public MainViewModel(IScreen hostScreen)
	    {
	        this.HostScreen = hostScreen; 
	        this.ChatList = new ReactiveList<ChatViewModel>();
	        this._client = Locator.Current.GetService<MumblerClient>();

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

	        this._client.PrivateChats.ItemsAdded.ObserveOn(RxApp.MainThreadScheduler).Subscribe(chat =>
	        {
                var vm = new ChatViewModel(this.HostScreen, ChatViewModelType.PrivateChat, chat);
                this.ChatList.Add(vm);
	            this.SelectedTab = vm;
	        });
	    }
	}
}
