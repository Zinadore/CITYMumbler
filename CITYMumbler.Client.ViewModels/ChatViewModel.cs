using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Client.ViewModels
{
	public class ChatViewModel : ReactiveObject, IRoutableViewModel
	{
		public string UrlPathSegment => "chatView";
		public IScreen HostScreen { get; }
		private MumblerClient _mumblerClient;
		private UserService _userService;
		private Client Me;

		public ReactiveCommand<Unit, Unit> SendCommand;

		public ChatViewModel(IScreen hostScreen)
		{
			this.HostScreen = hostScreen;
			this.SendCommand = ReactiveCommand.Create(SendMessage);
			_mumblerClient = Locator.Current.GetService<MumblerClient>();
			_userService = Locator.Current.GetService<UserService>();

			//Me = Locator.Current.GetService<UserService>().Me;
			_userService.SetMe(new Client(5, "Giorgaras"));
			Me = _userService.Me;
			Group group = new Group("supergroup", (ushort) 3, (ushort) 5, JoinGroupPermissionTypes.Free, 10 );
			group.UserList.Add(Me);
            //this.WhenAnyValue(x => x.ChatInput)
            //    .Select(x => x?.Trim())
            //    .ToProperty(this, x => x.ChatDisplay, out _chatDisplay);
        }

		private void SendMessage()
		{
			this.ChatDisplay += ("\n ME: " + ChatInput);
			_mumblerClient.SendGroupMessage((ushort) 5, ChatInput);
			this.ChatInput = "";
		}

		private readonly ObservableAsPropertyHelper<List<TabContentViewModel>> _tabList;
		public List<TabContentViewModel> TabList
		{
			get { return _tabList.Value; }
		}

		private string _chatDisplay;
		public string ChatDisplay
		{
			get { return _chatDisplay; }
			set { this.RaiseAndSetIfChanged(ref _chatDisplay, value); }
		}

		private string _chatInput;
		public string ChatInput
		{
			get { return _chatInput; }
			set { this.RaiseAndSetIfChanged(ref _chatInput, value); }
		}
	}
}
