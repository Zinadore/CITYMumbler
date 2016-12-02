using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Contracts.Services.Logger;
using ReactiveUI;
using Splat;
using ILogger = CITYMumbler.Common.Contracts.Services.Logger.ILogger;
using LogLevel = CITYMumbler.Common.Contracts.Services.Logger.LogLevel;

namespace CITYMumbler.Server.ViewModels
{
    public class MainViewModel: ReactiveObject, IRoutableViewModel
    {
        private int portNumeric;

        private ILogger logger;

        private bool _started;
        private bool Started
        {
            get { return _started; }
            set { this.RaiseAndSetIfChanged(ref _started, value); }
        }
        
        private CompositeDisposable _subscriptions;
        public static MumblerServer MumblerServer { get; private set; }
        public string UrlPathSegment => "main";
        public IScreen HostScreen { get; }
        private readonly ObservableAsPropertyHelper<bool> _isStartEnabled;
        public bool IsStartEnabled => _isStartEnabled.Value;

        public ReactiveCommand<Unit, Unit> StartCommand;
        public ReactiveCommand<Unit, Unit> StopCommand;


        private string _port;
        public string Port
        {
            get { return _port; }
            set { this.RaiseAndSetIfChanged(ref _port, value); }
        }

        public IObservable<LogEntry> Logs { get; private set; }



        public MainViewModel(IScreen host)
        {
            this.HostScreen = host;
            this.Logs = Locator.Current.GetService<ILoggerService>().Entries;
            this.logger = Locator.Current.GetService<ILoggerService>().GetLogger(this.GetType());
            MumblerServer = new MumblerServer(Locator.Current.GetService<ILoggerService>());
            this.Port = MumblerServer.Port.ToString();
            this._subscriptions = new CompositeDisposable();
            
            var sub = MumblerServer.IsRunning.Subscribe(running => this.Started = running);

            this._subscriptions.Add(sub);
            
            this.StartCommand = ReactiveCommand<Unit, Unit>.Create(StartServer);
            this.StopCommand = ReactiveCommand<Unit, Unit>.Create(StopServer);

            this.WhenAnyValue(x => x.Port, x => x.Started, (port, isStarted) => int.TryParse(Port, out portNumeric) && !isStarted)
                .ToProperty(this, vm => vm.IsStartEnabled, out _isStartEnabled);

            this.StartCommand.ThrownExceptions.Subscribe(ex =>
            {
                this.logger.Log(LogLevel.Error, ex.Message);
            });
        }

        private void StopServer()
        {
            MumblerServer.Stop();
        }

        private void StartServer()
        {
            MumblerServer.Start(this.portNumeric);
        }

        ~MainViewModel()
        {
            this._subscriptions.Dispose();
        }
    }
}
