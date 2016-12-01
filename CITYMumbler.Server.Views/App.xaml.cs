using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CITYMumbler.Common.Contracts.Services.Logger;
using CITYMumbler.Common.Services.Logger;
using CITYMumbler.Server.ViewModels;
using Splat;

namespace CITYMumbler.Server.Views
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private AppBootstraper _bootstraper;
        public ShellView ShellView { get; set; }

        public App()
        {
            Locator.CurrentMutable.RegisterLazySingleton(() => new LoggerService(), typeof(ILoggerService));
            this._bootstraper = new AppBootstraper();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.ShellView = new ShellView { ViewModel = this._bootstraper };
            this.ShellView.Show();
            this._bootstraper.Router.Navigate.Execute(Locator.Current.GetService<MainViewModel>());
        }
    }
}
