using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace CITYMumbler.Client.ViewModels
{
    public class CurrentChatUserListItemViewModel: ReactiveObject
    {
        private Client _client;
        private ushort _ownerId;

        public ushort ID => this._client.ID;

        public string Username => this._client.Name;

        private readonly ObservableAsPropertyHelper<string> _imagePath;
        public string ImagePath
        {
            get { return _imagePath.Value; }
        }

        public CurrentChatUserListItemViewModel(Client client, ushort ownerId)
        {
            this._client = client;
            this._ownerId = ownerId;

            this.WhenAnyValue(x => x._ownerId)
                .Select(ownerid => ownerid == this.ID)
                .Select(value => value ? "Images/crown.png" : "")
                .ToProperty(this, @this => @this.ImagePath, out this._imagePath);
        }
    }
}
