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
using Splat;

namespace CITYMumbler.Client.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Window, IViewFor<AppBootstrapper>
    {
        public ShellView(AppBootstrapper bootstrapper)
        {
            InitializeComponent();
            ViewModel = bootstrapper;
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
            this.Bind(ViewModel, vm => vm.Router, @this => @this.ContentHost.Router);
            this.ViewModel.Router.Navigate.Execute(Locator.Current.GetService<LoginViewModel>());
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (AppBootstrapper)value; }
        }
        public AppBootstrapper ViewModel { get; set; }
    }
}
