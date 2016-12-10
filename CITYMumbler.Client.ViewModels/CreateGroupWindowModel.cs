using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Networking.Contracts;
using ReactiveUI;

namespace CITYMumbler.Client.ViewModels
{
    public class CreateGroupWindowModel: ReactiveObject, ISupportsActivation
    {
        #region Private Members

        #endregion

        #region Reactive Properties

        private FriendlyEnum _selectedAuthentication;
        public FriendlyEnum SelectedAuthentication
        {
            get { return _selectedAuthentication; }
            set { this.RaiseAndSetIfChanged(ref _selectedAuthentication, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        private string _threshold;
        public string Threshold
        {
            get { return _threshold; }
            set { this.RaiseAndSetIfChanged(ref _threshold, value); }
        }
        #endregion

        #region Output Properties
        private bool _isCreateEnabled;
        public bool IsCreateEnabled
        {
            get { return _isCreateEnabled; }
            set { this.RaiseAndSetIfChanged(ref _isCreateEnabled, value); }
        }

        private bool _isNameValid;
        public bool IsNameValid
        {
            get { return _isNameValid; }
            set { this.RaiseAndSetIfChanged(ref _isNameValid, value); }
        }

        private bool _isPasswordEnabled;
        public bool IsPasswordEnabled
        {
            get { return _isPasswordEnabled; }
            set { this.RaiseAndSetIfChanged(ref _isPasswordEnabled, value); }
        }

        private bool _isPasswordValid;
        public bool IsPasswordValid
        {
            get { return _isPasswordValid; }
            set { this.RaiseAndSetIfChanged(ref _isPasswordValid, value); }
        }
        private bool _isThresholdValid;
        public bool IsThresholdValid
        {
            get { return _isThresholdValid; }
            set { this.RaiseAndSetIfChanged(ref _isThresholdValid, value); }
        }


        #endregion

        #region Public Members
        public FriendlyEnum[] AuthenticationOptions { get; private set; }
        private ViewModelActivator _activator;
        public ViewModelActivator Activator => _activator;
        public byte ThresholdNumeric;

        #endregion

        public CreateGroupWindowModel()
        {
            this._activator = new ViewModelActivator();
            this.AuthenticationOptions = new FriendlyEnum[]
            {
                new FriendlyEnum() { Type = JoinGroupPermissionTypes.Free, FriendlyName = "None"},
                new FriendlyEnum() { Type = JoinGroupPermissionTypes.Password, FriendlyName = "Password"}
            };

            this.SelectedAuthentication = this.AuthenticationOptions[0];

            this.WhenActivated(d =>
            {
                // Should the password field be activated
                d(this.WhenAnyValue(x => x.SelectedAuthentication)
                    .Select(x => x?.Type == JoinGroupPermissionTypes.Password)
                    .Do(x => Debug.WriteLine("Updating IsPasswordEnabled with {0}", x))
                    .Subscribe(value => this.IsPasswordEnabled = value));
                // Is the group name a valid string?
                d(this.WhenAnyValue(x => x.Name)
                    .Select(x => x?.Trim())
                    .DistinctUntilChanged()
                    .Select(x => !string.IsNullOrEmpty(x))
                    .Subscribe(value => this.IsNameValid = value));
                // Is the threshold a valid number? 255 seconds
                d(this.WhenAnyValue(x => x.Threshold)
                    .Select(x => x?.Trim())
                    .DistinctUntilChanged()
                    .Select(x => byte.TryParse(x, out ThresholdNumeric))
                    .Subscribe(value => this.IsThresholdValid = value));
                // Is the password valid if enabled?
                d(this.WhenAnyValue(x => x.Password, x => x.IsPasswordEnabled)
                    .Select(x => x.Item1?.Trim())
                    .Select(x => !string.IsNullOrEmpty(x) || !this.IsPasswordEnabled)
                    .Subscribe(value  => this.IsPasswordValid = value));

                d(this.WhenAnyValue(x => x.IsNameValid, x => x.IsPasswordValid, x => x.IsThresholdValid,
                    (name, pw, thresh) =>
                        name && pw && thresh)
                        .Subscribe(value => this.IsCreateEnabled = value));
            });
        }

        
    }

    public class FriendlyEnum
    {
        public JoinGroupPermissionTypes Type { get; set; }
        public string FriendlyName { get; set; }
    }
}
