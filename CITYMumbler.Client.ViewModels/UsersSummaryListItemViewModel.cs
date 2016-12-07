using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace CITYMumbler.Client.ViewModels
{
    public class UsersSummaryListItemViewModel: ReactiveObject
    {
        private readonly Client _client;

        public ushort ID => this._client.ID;

        public string Username => _client.Name;

        public event EventHandler<WhisperEventArgs> OnStartWhisper;

        public ReactiveCommand<Unit, Unit> WhisperCommand;

        public UsersSummaryListItemViewModel(Client client)
        {
            this._client = client;
            this.WhisperCommand = ReactiveCommand.Create(Whisper);
        }

        private void Whisper()
        {
            this.OnStartWhisper?.Invoke(this, new WhisperEventArgs(this._client.ID));
        }
    }

    public class WhisperEventArgs : EventArgs
    {
        public ushort WhisperID { get; private set; }

        public WhisperEventArgs(ushort id) { this.WhisperID = id; }
    }
}
