﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Client.ViewModels;
using CITYMumbler.Common.Contracts.Services.Logger;
using CITYMumbler.Common.Services.Logger;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Client.Views
{
    public class AppBootstrapper: ReactiveObject, IScreen
    {
        public RoutingState Router { get; }

		public AppBootstrapper() {
            this.Router = new RoutingState();
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));
			Locator.CurrentMutable.RegisterConstant(new LoggerService(), typeof(ILoggerService));
            
			Locator.CurrentMutable.RegisterConstant(new MumblerClient(), typeof(MumblerClient));

            RegisterViewModels();
            RegisterViews();
        }

        private void RegisterViewModels()
        {
            Locator.CurrentMutable.Register(() => new LoginViewModel(this), typeof(LoginViewModel));
			Locator.CurrentMutable.Register(() => new MainViewModel(this), typeof(MainViewModel));
			//Locator.CurrentMutable.Register(() => new ChatViewModel(this), typeof(ChatViewModel));
            Locator.CurrentMutable.Register(() => new LogWindowViewModel(), typeof(LogWindowViewModel));
            Locator.CurrentMutable.RegisterConstant(new SummaryViewModel(), typeof(SummaryViewModel));
		}

        private void RegisterViews()
        {
            Locator.CurrentMutable.Register(() => new LoginView(), typeof(IViewFor<LoginViewModel>));
			Locator.CurrentMutable.Register(() => new MainView(), typeof(IViewFor<MainViewModel>));
			Locator.CurrentMutable.Register(() => new ChatView(), typeof(IViewFor<ChatViewModel>));
            Locator.CurrentMutable.Register(() => new GroupsSummaryListItemView(), typeof(IViewFor<GroupsSummaryListItemViewModel>));
            Locator.CurrentMutable.Register(() => new UsersSummaryListItemView(), typeof(IViewFor<UsersSummaryListItemViewModel>));
            Locator.CurrentMutable.Register(() => new CurrentChatUserListItemView(), typeof(IViewFor<CurrentChatUserListItemViewModel>));
		}

    }
}
