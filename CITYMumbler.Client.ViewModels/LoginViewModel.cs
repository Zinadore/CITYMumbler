using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace CITYMumbler.Client.ViewModels
{
    public class LoginViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment => "login";
        public IScreen HostScreen { get; }

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
            get { return _error.Value; }
        }

        private ObservableAsPropertyHelper<bool> _canConnect;
        public bool CanConnect => _canConnect.Value;


        public LoginViewModel(IScreen host)
        {
            HostScreen = host;

            this.WhenAnyValue(x => x.Address)
                .Select(x => x?.Trim())
                .ToProperty(this, x => x.Error, out _error);

            this._canConnect = this.WhenAnyValue(x => x.Address, x => x.Port, x => x.Username,
                (address, port, username) => !string.IsNullOrWhiteSpace(address) && !string.IsNullOrWhiteSpace(port) &&
                                             !string.IsNullOrWhiteSpace(username))
                .ToProperty(this, @this => @this.CanConnect, false);

            this.WhenAnyValue(x => x.Address, x => x.Port, x => x.Username,
                (address, port, username) => new Credentials(address, port, username));
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
