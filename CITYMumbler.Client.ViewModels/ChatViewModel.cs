using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
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
	    private ChatViewModelType _type;
	    private ushort _filterId;
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

        public ushort ID { get; private set; }

	    public IObservable<ChatEntry> Entries { get; private set; }

		public ReactiveCommand<Unit, Unit> SendCommand;

		public ChatViewModel(IScreen hostScreen, ChatViewModelType type, ushort filterId)
		{
			this.HostScreen = hostScreen;
		    this._type = type;
		    this._filterId = filterId;
            this._mumblerClient = Locator.Current.GetService<MumblerClient>();
		    this.ID = _mumblerClient.ID;
            if (this._type == ChatViewModelType.GroupChat)
            {
                this.Header = string.Format("Group chat #{0}", _filterId);
		        this.Entries = this._mumblerClient.GroupMessages.Where(entry => entry.GroupId == this._filterId);
            }
            else
		    {
                this.Header = string.Format("Private chat #{0}", _filterId);
                this.Entries = this._mumblerClient.PrivateMessages.Where(entry => entry.SenderId == this._filterId);
		    }
			this.SendCommand = ReactiveCommand.Create(SendMessage);
            
        }

		private void SendMessage()
		{
		    if (this._type == ChatViewModelType.GroupChat)
		    {
		        this._mumblerClient.SendGroupMessage(this._filterId, ChatInput);
		    }
		    else
		    {
		        this._mumblerClient.SendPrivateMessage(this._filterId, ChatInput);
		    }
		    this.ChatInput = "";
		}
	}

    public enum ChatViewModelType
    {
        PrivateChat,
        GroupChat
    }
}
