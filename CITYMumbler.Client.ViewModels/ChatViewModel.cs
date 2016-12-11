using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Client.ViewModels
{
	public class ChatViewModel : ReactiveObject, IRoutableViewModel
	{
        #region Private Members
        private MumblerClient _mumblerClient;
        private ushort _filterId;
	    private Client _localClient;
	    private Client _remoteClient;
        #endregion

        #region Reactive Properties
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
	    private string _header;
	    public string Header
	    {
	        get { return _header; }
	        set { this.RaiseAndSetIfChanged(ref _header, value); }
	    }


        #endregion

        public string UrlPathSegment => "chatView";
		public IScreen HostScreen { get; }

        public ushort LocalID { get; private set; }
        public ChatViewModelType ChatType { get; private set; }

        public ushort RemoteID => this._filterId;

        public Group Group { get; private set; }

        public IObservable<ChatEntry> Entries { get; private set; }

		public ReactiveCommand<Unit, Unit> SendCommand;
	    public ReactiveCommand<Unit, Unit> CloseCommand;

	    public ChatViewModel(IScreen hostScreen, ChatViewModelType chatType, PrivateChat chat):this(hostScreen, chatType, chat.RemoteUser.ID)
	    {
	        this._localClient = chat.LocalUser;
	        this._remoteClient = chat.RemoteUser;
	        this.Header = chat.RemoteUser.Name;
	    }

        public ChatViewModel(IScreen hostScreen, ChatViewModelType chatType, Group group):this(hostScreen, chatType, group.ID)
        {
            this.Group = group;
	        this.Header = group.Name;
	    }

        private ChatViewModel(IScreen hostScreen, ChatViewModelType chatType, ushort filterId)
		{
			this.HostScreen = hostScreen;
		    this.ChatType = chatType;
		    this._filterId = filterId;
            this._mumblerClient = Locator.Current.GetService<MumblerClient>();
		    this.LocalID = _mumblerClient.ID;
            if (this.ChatType == ChatViewModelType.GroupChat)
            {
		        this.Entries = this._mumblerClient.GroupMessages.Where(entry => entry.GroupId == this._filterId);
            }
            else
		    {
                this.Entries = this._mumblerClient.PrivateMessages.Where(entry => entry.SenderId == this._filterId ||  (entry.SenderId == this._localClient.ID && entry.ReceiverId == this._filterId));
		    }
			this.SendCommand = ReactiveCommand.Create(SendMessage);
            this.CloseCommand = ReactiveCommand.Create(LeaveConversation);

		}

	    private void LeaveConversation()
	    {
	        if (this.ChatType == ChatViewModelType.GroupChat)
	            this._mumblerClient.LeaveGroup(_filterId);
	        else
                this._mumblerClient.CloseWhisper(_filterId);
	    }

	    private void SendMessage()
		{
		    if (this.ChatType == ChatViewModelType.GroupChat)
		    {
		        this._mumblerClient.SendGroupMessage(this._filterId, ChatInput);
		    }
		    else
		    {
		        this._mumblerClient.SendPrivateMessage(this._filterId, ChatInput);
		    }
		    this.ChatInput = "";
		}

	    public ReactiveList<Client> GetUsers()
	    {
	        if (this.ChatType == ChatViewModelType.GroupChat)
	            return this.Group.GroupUsers;
            var returnList = new ReactiveList<Client>();
            returnList.Add(this._localClient);
            returnList.Add(this._remoteClient);
            return returnList;
	    }
	}

    public enum ChatViewModelType
    {
        PrivateChat,
        GroupChat
    }
}
