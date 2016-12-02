using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Contracts.Services.Logger;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Client.ViewModels
{
    public class LoginViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment => "login";
        public IScreen HostScreen { get; }
		public ReactiveCommand<Unit, Unit> ConnnectCommand;

		private MumblerClient _mumblerClient;
		private int _portNumeric;
		private IPAddress _addressIp;

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

        private readonly ObservableAsPropertyHelper<string> _error;
        public string Error
        {
            get { return "error default"; }
        }

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

		public LoginViewModel(IScreen host)
        {
			this.Address  = "127.0.0.1";
			this.Port = "21992";
			HostScreen = host;
			_mumblerClient = Locator.Current.GetService<MumblerClient>();

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

			var CanExecuteConnect = this.WhenAnyValue(x => x.IsAddressValid, x => x.IsPortValid, x => x.IsUsernameValid,
				(address, port, username) => address && port && username);

			CanExecuteConnect.ToProperty(this, @this => @this.IsConnectButtonEnabled, out _isConnectButtonEnabled);
			//this._canConnect = this.WhenAnyValue(x => x.Address, x => x.Port, x => x.Username,
			//    (address, port, username) => !string.IsNullOrWhiteSpace(address) && !string.IsNullOrWhiteSpace(port) &&
			//                                 !string.IsNullOrWhiteSpace(username) && int.TryParse(Port, out _portNumberic) && IPAddress.TryParse(address, out _addressIp))
			//    .ToProperty(this, @this => @this.CanConnect, false);

			this.ConnnectCommand = ReactiveCommand.Create(Connect, CanExecuteConnect);

			//this.WhenAnyValue(x => x.Address, x => x.Port, x => x.Username,
			//    (address, port, username) => new Credentials(address, port, username));
		}

	    private void Connect()
	    {
			_mumblerClient.Connect(_addressIp, _portNumeric, Username);
		}
    }

    struct Credentials
    {
        public string Address { get; private set; }
        public int Port { get; private set; }
        public string Username { get; private set; }

        public Credentials(string address, string port, string username)
        {
            Address = address;
            Port = Int32.Parse(port);
            Username = username;
        }

    }
}
