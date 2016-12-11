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
        private int timeoutThreshold;

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

        private string _threshold;
        public string Threshold
        {
            get { return _threshold; }
            set { this.RaiseAndSetIfChanged(ref _threshold, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _isPortValid;
        public bool IsPortValid
        {
            get { return _isPortValid.Value; }
        }

        private readonly ObservableAsPropertyHelper<bool> _isThresholdValid;
        public bool IsThresholdValid
        {
            get { return _isThresholdValid.Value; }
        }

        private readonly ObservableAsPropertyHelper<bool> _isStopEnabled;
        public bool IsStopEnabled
        {
            get { return _isStopEnabled.Value; }
        }
        public IObservable<LogEntry> Logs { get; private set; }

        public MainViewModel(IScreen host)
        {
            this.HostScreen = host;
            this.Logs = Locator.Current.GetService<ILoggerService>().Entries;
            this.logger = Locator.Current.GetService<ILoggerService>().GetLogger(this.GetType());
            MumblerServer = new MumblerServer(Locator.Current.GetService<ILoggerService>());
            this.Port = MumblerServer.Port.ToString();
            this.Threshold = 60.ToString();
            this._subscriptions = new CompositeDisposable();
            
            var sub = MumblerServer.IsRunning.Subscribe(running => this.Started = running);

            this._subscriptions.Add(sub);

            this.WhenAnyValue(x => x.Port)
                .Select(x => x?.Trim())
                .DistinctUntilChanged()
                .Select(x => int.TryParse(x, out portNumeric))
                .ToProperty(this, @this => @this.IsPortValid, out _isPortValid);

            this.WhenAnyValue(x => x.Threshold)
                .Select(x => x?.Trim())
                .DistinctUntilChanged()
                .Select(x => int.TryParse(x, out timeoutThreshold))
                .ToProperty(this, @this => @this.IsThresholdValid, out _isThresholdValid);

            this.WhenAnyValue(x => x.IsPortValid, x => x.IsThresholdValid, x => x.Started,
                (port, threshold, started) => port && threshold && !started)
                .ToProperty(this, @this => @this.IsStartEnabled, out _isStartEnabled);

            this.WhenAnyValue(x => x.Started)
                .ToProperty(this, @this => @this.IsStopEnabled, out _isStopEnabled);
            
            this.StartCommand = ReactiveCommand.Create(StartServer);
            this.StopCommand = ReactiveCommand.Create(StopServer);

            this.WhenAnyValue(x => x.Port, x => x.Threshold, x => x.Started, (port, threshold, isStarted) => int.TryParse(Port, out portNumeric) && int.TryParse(Threshold, out timeoutThreshold) && !isStarted)
                .ToProperty(this, vm => vm.IsStartEnabled, out _isStartEnabled);


            this.StartCommand.ThrownExceptions.Subscribe(ex =>
            {
                this.logger.Log(LogLevel.Error, ex.Message);
            });

            this.StopCommand.ThrownExceptions.Subscribe(ex =>
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
            MumblerServer.Start(this.portNumeric, this.timeoutThreshold);
        }

        ~MainViewModel()
        {
            this._subscriptions.Dispose();
        }
    }
}
