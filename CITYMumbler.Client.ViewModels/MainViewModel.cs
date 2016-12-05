using System;
using System.Collections.Generic;
using System.Linq;
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
		public ReactiveList<ChatViewModel> ChatList { get; private set; }

	    public MainViewModel(IScreen hostScreen)
	    {
	        this.HostScreen = hostScreen; 
	        this.ChatList = new ReactiveList<ChatViewModel>();
	        this._client = Locator.Current.GetService<MumblerClient>();
	        this._client.OnGroupsReceived += (s, e) =>
	        {
	            this._client.JoinGroup(1);
	        };
            //this._client.JoinGroup(1);
	        this._client.JoinedGroups.ItemsAdded.Subscribe(group =>
	        {
	            this.ChatList.Add(new ChatViewModel(this.HostScreen, ChatViewModelType.GroupChat, group.ID));
	        });
	    }
	}
}
