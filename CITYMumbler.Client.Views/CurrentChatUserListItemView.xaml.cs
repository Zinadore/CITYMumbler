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
    /// Interaction logic for CurrentChatUserListItemView.xaml
    /// </summary>
    public partial class CurrentChatUserListItemView : UserControl, IViewFor<CurrentChatUserListItemViewModel>
    {
        public CurrentChatUserListItemView()
        {
            InitializeComponent();
            //this.OneWayBind(ViewModel, vm => vm.ImagePath, @this => @this.AdminImage.Source, getImage);
            this.OneWayBind(ViewModel, vm => vm.Username, @this => @this.Username.Text);
        }

        private BitmapImage getImage(string path)
        {
            var uri = new Uri(path);
            var img = new BitmapImage(uri);
            return img;
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (CurrentChatUserListItemViewModel)value; }
        }
        public CurrentChatUserListItemViewModel ViewModel { get; set; }
    }
}
