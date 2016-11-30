using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace CITYMumbler.Client.ViewModels
{
	public class ChatViewModel : ReactiveObject, IRoutableViewModel
	{
		public string UrlPathSegment => "chatView";
		public IScreen HostScreen { get; }


		public ChatViewModel(IScreen hostScreen) { this.HostScreen = hostScreen; }
	}
}
