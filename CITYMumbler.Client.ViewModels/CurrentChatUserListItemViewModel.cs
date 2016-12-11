using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace CITYMumbler.Client.ViewModels
{
    public class CurrentChatUserListItemViewModel : ReactiveObject
    {
        private Client _remoteClient;
        private MumblerClient _localClient;
        private Group _group;

        public ushort ID => this._remoteClient.ID;

        public string Username => this._remoteClient.Name;

        private readonly ObservableAsPropertyHelper<bool> _isOwner;
        public bool IsOwner

        {
            get { return _isOwner.Value; }
        }

        public ReactiveCommand<Unit, Unit> KickCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> WhisperCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> PromoteCommand { get; private set; }


        public CurrentChatUserListItemViewModel(Client remoteClient, MumblerClient localClient, Group group)
        {
            this._remoteClient = remoteClient;
            this._localClient = localClient;
            this._group = group;

            this.WhenAnyValue(x => x._localClient, x => x._group.OwnerID, (c, g) => c)
                .Select(x => x.ID == this._group.OwnerID)
                .ToProperty(this, @this => @this.IsOwner, out this._isOwner);


            this.KickCommand = ReactiveCommand.Create(() =>
            {
                if (this._remoteClient.ID == this._localClient.ID)
                    return;
                this._localClient.Kick(this._group.ID, this._remoteClient.ID);
            });

            this.WhisperCommand = ReactiveCommand.Create(() =>
            {
                this._localClient.Whisper(this._remoteClient.ID);
            });

            this.PromoteCommand = ReactiveCommand.Create(() =>
            {
                if (this._remoteClient.ID == this._localClient.ID)
                    return;
                this._localClient.ChangeGroupOwner(this._group.ID, this._remoteClient.ID);
            });
        }
    }
}
