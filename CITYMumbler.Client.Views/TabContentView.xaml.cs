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

namespace CITYMumbler.Client.Views
{
	/// <summary>
	/// Interaction logic for TabContentView.xaml
	/// </summary>
	public partial class TabContentView : UserControl, IViewFor<TabContentViewModel>
	{
		public TabContentView()
		{
			InitializeComponent();
			this.OneWayBind(this.ViewModel, vm => vm.ChatDisplay, @this => @this.ChatDisplay.Text);
			this.Bind(this.ViewModel, vm => vm.ChatInput, @this => @this.ChatInput.Text);
		}

		object IViewFor.ViewModel
		{
			get { return ViewModel; }
			set { ViewModel = (TabContentViewModel) value; }
		}
		public TabContentViewModel ViewModel { get; set; }
	}
}
