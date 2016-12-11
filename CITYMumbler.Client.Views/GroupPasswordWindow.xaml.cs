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
using System.Windows.Shapes;
using CITYMumbler.Client.ViewModels;
using ReactiveUI;

namespace CITYMumbler.Client.Views
{
    /// <summary>
    /// Interaction logic for GroupPasswordWindow.xaml
    /// </summary>
    public partial class GroupPasswordWindow : Window, IViewFor<GroupPasswordWindowModel>
    {
        public GroupPasswordWindow()
        {
            InitializeComponent();
            this.Bind(ViewModel, vm => vm.Password, @this => @this.Password.Text);
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (GroupPasswordWindowModel)value; }
        }
        public GroupPasswordWindowModel ViewModel { get; set; }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
