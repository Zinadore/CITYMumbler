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
    /// Interaction logic for UsersSummaryListItemView.xaml
    /// </summary>
    public partial class UsersSummaryListItemView : UserControl, IViewFor<UsersSummaryListItemViewModel>
    {
        public UsersSummaryListItemView()
        {
            InitializeComponent();
            this.Bind(ViewModel, vm => vm.Username, @this => @this.Username.Text);
            this.WhenActivated(d =>
            {
                d(this.BindCommand(ViewModel, vm => vm.WhisperCommand, @this => @this.MenuItemWhisper));
            });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (UsersSummaryListItemViewModel)value; }
        }
        public UsersSummaryListItemViewModel ViewModel { get; set; }
    }
}
