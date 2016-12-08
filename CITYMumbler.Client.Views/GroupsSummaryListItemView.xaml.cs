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
    /// Interaction logic for GroupsSummaryListItemView.xaml
    /// </summary>
    public partial class GroupsSummaryListItemView : UserControl, IViewFor<GroupsSummaryListItemViewModel>
    {
        public GroupsSummaryListItemView()
        {
            InitializeComponent();
            
            this.WhenActivated(d =>
            {
                d(this.OneWayBind(ViewModel, vm => vm.GroupName, @this => @this.GroupName.Text));
                d(this.BindCommand(ViewModel, vm => vm.JoinGroupCommand, @this => @this.ContextMenuJoin));
                d(this.BindCommand(ViewModel, vm => vm.LeaveGroupCommand, @this => @this.ContextMenuLeave));
                d(this.OneWayBind(ViewModel, vm => vm.IsJoined, @this => @this.ContextMenuJoin.Visibility, value => value ? Visibility.Collapsed : Visibility.Visible));
                d(this.OneWayBind(ViewModel, vm => vm.IsJoined, @this => @this.ContextMenuLeave.Visibility, value => value ? Visibility.Visible : Visibility.Collapsed));
            });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (GroupsSummaryListItemViewModel)value; }
        }
        public GroupsSummaryListItemViewModel ViewModel { get; set; }
    }
}
