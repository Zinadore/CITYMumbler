using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace CITYMumbler.Client.ViewModels
{
	public class MainViewModel : ReactiveObject, IRoutableViewModel
	{
		public string UrlPathSegment => "mainView";
		public IScreen HostScreen { get; }
		
		public MainViewModel(IScreen hostScreen) { this.HostScreen = hostScreen; }
	}
}
