using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CITYMumbler.Common.Contracts.Services.Logger;
using ReactiveUI;
using Splat;
using ILogger = CITYMumbler.Common.Contracts.Services.Logger.ILogger;
using LogLevel = CITYMumbler.Common.Contracts.Services.Logger.LogLevel;

namespace CITYMumbler.Client.ViewModels
{
    public class LoginViewModel : ReactiveObject, IRoutableViewModel
    {
        #region Private Members
        private MumblerClient _mumblerClient;
        private int _portNumeric;
        private IPAddress _addressIp;
        private IObservable<bool> CanExecuteConnect;
        private ILogger logger;
        #endregion

        #region IRoutableViewModel
        public string UrlPathSegment => "login";
        public IScreen HostScreen { get; }
        #endregion

        #region Output Properties
        private ObservableAsPropertyHelper<bool> _isConnectButtonEnabled;
        public bool IsConnectButtonEnabled => _isConnectButtonEnabled.Value;

        private readonly ObservableAsPropertyHelper<bool> _isAddressValid;
        public bool IsAddressValid
        {
            get { return _isAddressValid.Value; }
        }

        private readonly ObservableAsPropertyHelper<bool> _isPortValid;
        public bool IsPortValid
        {
            get { return _isPortValid.Value; }
        }

        private readonly ObservableAsPropertyHelper<bool> _isUsernameValid;
        public bool IsUsernameValid
        {
            get { return _isUsernameValid.Value; }
        }
        #endregion

        #region Reactive Properties
        private string _address;
        public string Address
        {
            get { return _address; }
            set { this.RaiseAndSetIfChanged(ref _address, value); }
        }

        private string _port;
        public string Port
        {
            get { return _port; }
            set { this.RaiseAndSetIfChanged(ref _port, value); }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { this.RaiseAndSetIfChanged(ref _username, value); }
        }
        #endregion

        public ReactiveCommand<Unit, Unit> ConnnectCommand;

        public IObservable<LogEntry> Logs { get; private set; }
	    

		public LoginViewModel(IScreen host)
        {
			this.Address  = "127.0.0.1";
			this.Port = "21992";
			this.HostScreen = host;
			this._mumblerClient = Locator.Current.GetService<MumblerClient>();
            this._mumblerClient.OnConnected += MumblerClient_OnConnected;
		    this.logger = Locator.Current.GetService<ILoggerService>().GetLogger(this.GetType());

		    this.Logs = Locator.Current.GetService<ILoggerService>().Entries;

			this.WhenAnyValue(x => x.Address)
			    .Select(x => x?.Trim())
				.DistinctUntilChanged()
			    .Select(x => IPAddress.TryParse(x, out _addressIp))
			    .ToProperty(this, @this => @this.IsAddressValid, out _isAddressValid, false);

			this.WhenAnyValue(x => x.Port)
				.Select(x => x?.Trim())
				.DistinctUntilChanged()
				.Select(x => int.TryParse(Port, out _portNumeric))
				.ToProperty(this, @this => @this.IsPortValid, out _isPortValid, false);

			this.WhenAnyValue(x => x.Username)
			    .Select(x => x?.Trim())
				.DistinctUntilChanged()
			    .Select(x => !string.IsNullOrEmpty(x))
			    .ToProperty(this, @this => @this.IsUsernameValid, out _isUsernameValid, false);

		    this.CanExecuteConnect = this.WhenAnyValue(x => x.IsAddressValid, x => x.IsPortValid, x => x.IsUsernameValid,
		        (address, port, username) => address && port && username);

			this.CanExecuteConnect.ToProperty(this, @this => @this.IsConnectButtonEnabled, out _isConnectButtonEnabled);

			this.ConnnectCommand = ReactiveCommand.Create(Connect, CanExecuteConnect);
		    this.ConnnectCommand.ThrownExceptions.Subscribe(ex =>
		    {
		        this.logger.Log(LogLevel.Error, ex.Message);
		    });

		    this.HostScreen.Router.Navigate.ThrownExceptions.Subscribe(ex =>
		    {
		        this.logger.Log(LogLevel.Error, ex.Message);
		        Debug.WriteLine(ex.StackTrace);
		    });
        }

        private void MumblerClient_OnConnected(object sender, EventArgs eventArgs)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var vm = Locator.Current.GetService<MainViewModel>();
                this.HostScreen.Router.Navigate.Execute(vm);
            }));
        }

        private void Connect()
	    {
			_mumblerClient.Connect(_addressIp, _portNumeric, Username);
		}
    }
}
