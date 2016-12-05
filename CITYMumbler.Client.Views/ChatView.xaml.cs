using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CITYMumbler.Client.ViewModels;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Client.Views
{
	/// <summary>
	/// Interaction logic for ChatView.xaml
	/// </summary>
	public partial class ChatView : UserControl, IViewFor<ChatViewModel>
	{
		public ChatView()
		{
			InitializeComponent();
			this.Bind(this.ViewModel, vm => vm.ChatInput, @this => @this.ChatInput.Text);

			this.BindCommand(ViewModel, vm => vm.SendCommand, @this => @this.SendButton);

			
		}

		object IViewFor.ViewModel
		{
			get { return ViewModel; }
			set { ViewModel = (ChatViewModel) value; }
		}
		public ChatViewModel ViewModel { get; set; }
	}
}
