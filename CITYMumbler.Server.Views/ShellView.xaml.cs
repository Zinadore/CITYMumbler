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
using CITYMumbler.Server.ViewModels;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Server.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window, IViewFor<AppBootstraper>
    {
        public ShellView()
        {
            InitializeComponent();
            this.Bind(ViewModel, vm => vm.Router, @this => @this.ContentHost.Router);
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (AppBootstraper)value; }
        }
        public AppBootstraper ViewModel { get; set; }
    }
}
