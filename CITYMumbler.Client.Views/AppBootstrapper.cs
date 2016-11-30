using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Client.ViewModels;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Client.Views
{
    public class AppBootstrapper: ReactiveObject, IScreen
    {
        public ReactiveObject ActiveModel { get; set; }
        public RoutingState Router { get; }

        public AppBootstrapper() {
            this.Router = new RoutingState();
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));

            RegisterViewModels();
            RegisterViews();
        }

        private void RegisterViewModels()
        {
            Locator.CurrentMutable.Register(() => new LoginViewModel(this), typeof(LoginViewModel));
        }

        private void RegisterViews()
        {
            Locator.CurrentMutable.Register(() => new LoginView(), typeof(IViewFor<LoginViewModel>));
        }

    }
}
