﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class MainView : UserControl, IViewFor<MainViewModel>
	{
		public MainView()
		{
			InitializeComponent();
		    this.OneWayBind(ViewModel, vm => vm.ChatList, @this => @this.ChatTabControl.ItemsSource);
		    this.OneWayBind(ViewModel, vm => vm.MyUsername, @this => @this.MyUsername.Text);
		    this.Bind(ViewModel, vm => vm.SelectedTab, @this => @this.ChatTabControl.SelectedItem);
		    this.OneWayBind(ViewModel, vm => vm.CurrentUsersVMs, @this => @this.CurrentUsers.ItemsSource);
		    this.BindCommand(ViewModel, vm => vm.CreateGroupCommand, @this => @this.NewGroupButton);
		    this.WhenActivated(d =>
		    {
		        d(this.ViewModel.CreateGroupInteraction.RegisterHandler(interaction =>
		        {
		            var dialog = new NewGroupWindow(interaction.Input) { Owner = Window.GetWindow(this)};
		            var result = dialog.ShowDialog();
                    interaction.SetOutput(result.Value);
		        }));
		    });
		}

		object IViewFor.ViewModel
		{
			get { return ViewModel; }
			set { ViewModel = (MainViewModel) value; }
		}
		public MainViewModel ViewModel { get; set; }
    }
}
