using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using CITYMumbler.Client.ViewModels;
using ReactiveUI;

namespace CITYMumbler.Client.Views
{
    /// <summary>
    /// Interaction logic for NewGroupWindow.xaml
    /// </summary>
    public partial class NewGroupWindow : Window, IViewFor<CreateGroupWindowModel>
    {
        public NewGroupWindow(CreateGroupWindowModel viewModel)
        {
            InitializeComponent();
            this.ViewModel = viewModel;
            this.WhenActivated(d =>
            {
                d(this.Bind(ViewModel, vm => vm.Name, @this => @this.Name.Text));
                d(this.Bind(ViewModel, vm => vm.Threshold, @this => @this.Threshold.Text));
                d(this.OneWayBind(ViewModel, vm => vm.AuthenticationOptions, @this => @this.Authentication.ItemsSource));
                d(this.Bind(ViewModel, vm => vm.SelectedAuthentication, @this => @this.Authentication.SelectedItem));
                d(this.OneWayBind(ViewModel, vm => vm.IsPasswordEnabled, @this => @this.Password.IsEnabled));
                d(this.Bind(ViewModel, vm => vm.Password, @this => @this.Password.Text));
                d(this.Bind(ViewModel, vm => vm.IsCreateEnabled, @this => @this.CreateButton.IsEnabled));
            });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (CreateGroupWindowModel)value; }
        }
        public CreateGroupWindowModel ViewModel { get; set; }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
