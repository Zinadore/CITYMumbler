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
		private List<TabItem> _tabItems;
		private TabItem _tabAdd;

		public ChatView()
		{
			//try
			//{
				InitializeComponent();
				ViewModel = Locator.Current.GetService<ChatViewModel>();
				this.OneWayBind(this.ViewModel, vm => vm.ChatDisplay, @this => @this.ChatDisplay.Text);
				this.Bind(this.ViewModel, vm => vm.ChatInput, @this => @this.ChatInput.Text);

			this.BindCommand(ViewModel, vm => vm.SendCommand, @this => @this.SendButton);

			// initialize tabItem array
			//_tabItems = new List<TabItem>();

			// add a tabItem with + in header 
			//TabItem tabAdd = new TabItem();
			//tabAdd.Header = "+";

			//_tabItems.Add(tabAdd);

			// add first tab
			//this.AddTabItem();

			// bind tab control
			//tabDynamic.DataContext = _tabItems;
			//this.OneWayBind(this.ViewModel, vm => vm.TabList, @this => @this.TabDynamic.ItemsSource);

			//TabDynamic.SelectedIndex = 0;
			//}
			//catch (Exception ex)
			//{
			//	MessageBox.Show(ex.Message);
			//}
		}

		//private TabItem AddTabItem()
		//{
		//	int count = _tabItems.Count;

		//	// create new tab item
		//	TabItem tab = new TabItem();
		//	tab.Header = string.Format("Tab {0}", count);
		//	tab.Name = string.Format("tab{0}", count);
		//	tab.HeaderTemplate = TabDynamic.FindResource("TabHeader") as DataTemplate;

		//	// add controls to tab item, this case I added just a textbox
		//	//TextBox txt = new TextBox();
		//	//txt.Name = "txt";
		//	//tab.Content = txt;

		//	// insert tab item right before the last (+) tab item
		//	_tabItems.Insert(count - 1, tab);
		//	return tab;
		//}

		//private void tabDynamic_SelectionChanged(object sender, SelectionChangedEventArgs e)
		//{
		//	TabItem tab = TabDynamic.SelectedItem as TabItem;

		//	if (tab != null && tab.Header != null)
		//	{
		//		if (tab.Header.Equals("+"))
		//		{
		//			// clear tab control binding
		//			TabDynamic.DataContext = null;

		//			// add new tab
		//			TabItem newTab = this.AddTabItem();

		//			// bind tab control
		//			TabDynamic.DataContext = _tabItems;

		//			// select newly added tab item
		//			TabDynamic.SelectedItem = newTab;
		//		}
		//		else
		//		{
		//			// your code here...
		//		}
		//	}
		//}

		//private void btnDelete_Click(object sender, RoutedEventArgs e)
		//{
		//	string tabName = (sender as Button).CommandParameter.ToString();

		//	var item = TabDynamic.Items.Cast<TabItem>().SingleOrDefault(i => i.Name.Equals(tabName));

		//	TabItem tab = item as TabItem;

		//	if (tab != null)
		//	{
		//		if (_tabItems.Count < 3)
		//		{
		//			MessageBox.Show("Cannot remove last tab.");
		//		}
		//		else if (MessageBox.Show(string.Format("Are you sure you want to remove the tab '{0}'?", tab.Header.ToString()),
		//	"Remove Tab", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
		//		{
		//			// get selected tab
		//			TabItem selectedTab = TabDynamic.SelectedItem as TabItem;

		//			// clear tab control binding
		//			TabDynamic.DataContext = null;

		//			_tabItems.Remove(tab);

		//			// bind tab control
		//			TabDynamic.DataContext = _tabItems;

		//			// select previously selected tab. if that is removed then select first tab
		//			if (selectedTab == null || selectedTab.Equals(tab))
		//			{
		//				selectedTab = _tabItems[0];
		//			}
		//			TabDynamic.SelectedItem = selectedTab;
		//		}
		//	}
		//}

		object IViewFor.ViewModel
		{
			get { return ViewModel; }
			set { ViewModel = (ChatViewModel) value; }
		}
		public ChatViewModel ViewModel { get; set; }
	}
}
