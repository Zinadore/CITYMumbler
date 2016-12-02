using CITYMumbler.Common.Contracts.Services.Logger;
using CITYMumbler.Common.Services.Logger;
using CITYMumbler.Server.ViewModels;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Server.Views
{
    public class AppBootstraper: ReactiveObject, IScreen
    {
        public RoutingState Router { get; }
        
        public AppBootstraper() {
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));
            Router = Router ?? new RoutingState();
            Locator.CurrentMutable.RegisterLazySingleton(() => new LoggerService(), typeof(ILoggerService));
            RegisterViewModels();
            RegisterViews();
        }

        private void RegisterViewModels()
        {
            Locator.CurrentMutable.Register(() => new MainViewModel(this), typeof(MainViewModel));
        }

        private void RegisterViews()
        {
            Locator.CurrentMutable.Register(() => new MainView(), typeof(IViewFor<MainViewModel>));
        }

    }
}
