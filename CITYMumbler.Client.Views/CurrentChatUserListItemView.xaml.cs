using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
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
using ReactiveUI.Legacy;

namespace CITYMumbler.Client.Views
{
    /// <summary>
    /// Interaction logic for CurrentChatUserListItemView.xaml
    /// </summary>
    public partial class CurrentChatUserListItemView : UserControl, IViewFor<CurrentChatUserListItemViewModel>
    {
        public CurrentChatUserListItemView()
        {
            InitializeComponent();
            this.OneWayBind(ViewModel, vm => vm.Username, @this => @this.Username.Text);
            this.OneWayBind(ViewModel, vm => vm.IsOwner, @this => @this.KickMenuItem.Visibility,
                value => value ? Visibility.Visible : Visibility.Collapsed);
            this.BindCommand(ViewModel, vm => vm.KickCommand, @this => @this.KickMenuItem);
            this.BindCommand(ViewModel, vm => vm.PromoteCommand, @this => @this.PromoteMenuItem);
            this.BindCommand(ViewModel, vm => vm.WhisperCommand, @this => @this.WhisperMenuItem);
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (CurrentChatUserListItemViewModel)value; }
        }
        public CurrentChatUserListItemViewModel ViewModel { get; set; }
    }
}
