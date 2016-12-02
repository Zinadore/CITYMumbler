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
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl, IViewFor<LoginViewModel>
    {
        public LoginView()
        {
            InitializeComponent();
            this.Bind(this.ViewModel, vm => vm.Address, @this => @this.Address.Text);
            this.Bind(this.ViewModel, vm => vm.Port, @this => @this.Port.Text);
            this.Bind(this.ViewModel, vm => vm.Username, @this => @this.Username.Text);
            this.OneWayBind(this.ViewModel, vm => vm.Error, @this => @this.Error.Text);
            this.OneWayBind(this.ViewModel, vm => vm.CanConnect, @this => @this.ConnectButton.IsEnabled);
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (LoginViewModel)value; }
        }
        public LoginViewModel ViewModel { get; set; }
    }
}
