using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace CITYMumbler.Client.ViewModels
{
	public class TabContentViewModel : ReactiveObject, IRoutableViewModel
	{
		//TODO: Define what am i modeling for
		public string UrlPathSegment { get; }
		public IScreen HostScreen { get; }

		public TabContentViewModel(IScreen hostScreen)
		{
			this.HostScreen = hostScreen;
			this.WhenAnyValue(x => x.ChatInput)
				.Select(x => x?.Trim())
				.ToProperty(this, x => x.ChatDisplay, out _chatDisplay);
		}

		private readonly ObservableAsPropertyHelper<string> _chatDisplay;
		public string ChatDisplay
		{
			get { return _chatDisplay.Value; }
		}

		private string _chatInput;
		public string ChatInput
		{
			get { return _chatInput; }
			set { this.RaiseAndSetIfChanged(ref _chatInput, value); }
		}
	}
}
