using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
using System.Windows.Threading;
using CITYMumbler.Client.ViewModels;
using CITYMumbler.Common.Contracts.Services.Logger;
using ReactiveUI;

namespace CITYMumbler.Client.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl, IViewFor<LoginViewModel>
    {
        private CompositeDisposable _subscriptions;
        public LoginView()
        {
            InitializeComponent();
            //this.Username.Focusable = true;
            //this.Username.Focus();
            //Keyboard.Focus(this.Username);
            this._subscriptions = new CompositeDisposable();
            this.Bind(this.ViewModel, vm => vm.Address, @this => @this.Address.Text);
            this.Bind(this.ViewModel, vm => vm.Port, @this => @this.Port.Text);
            this.Bind(this.ViewModel, vm => vm.Username, @this => @this.Username.Text);
            this.OneWayBind(this.ViewModel, vm => vm.IsConnectButtonEnabled, @this => @this.ConnectButton.IsEnabled);

	        this.BindCommand(this.ViewModel, vm => vm.ConnnectCommand, @this => @this.ConnectButton);

            IDisposable sub;
            this.WhenActivated(d =>
            {
                d(sub = this.ViewModel.Logs
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Subscribe(entry =>
                            {
                                this.Log.Foreground = getColorForLevel(entry.Level);
                                this.Log.Text = entry.Message;
                            }));
                this._subscriptions.Add(sub);
            });
                          
                //this.WhenAnyObservable(x => x.ViewModel.Logs)
                //          .SubscribeOn(RxApp.MainThreadScheduler)
                //          .Subscribe(entry =>
                //          {
                //              this.Log.Dispatcher.BeginInvoke(new Action(() =>
                //              {
                //                  this.Log.Foreground = getColorForLevel(entry.Level);
                //                  this.Log.Text = entry.Message;
                //              }));
                //          });

        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (LoginViewModel)value; }
        }
        public LoginViewModel ViewModel { get; set; }

        ~LoginView()
        {
            this._subscriptions.Dispose();
        }

        private Brush getColorForLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return Brushes.Chocolate;
                case LogLevel.Info:
                    return Brushes.CornflowerBlue;
                case LogLevel.Warn:
                    return Brushes.Orange;
                case LogLevel.Error:
                    return Brushes.Red;
                default:
                    return Brushes.AliceBlue;
            }
        }

        private void LoginView_OnLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(() =>
            {
                this.Username.Focus();
                Keyboard.Focus(this.Username);
            }));
        }
    }
}
