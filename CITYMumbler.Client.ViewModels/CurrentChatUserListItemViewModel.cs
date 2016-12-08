using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace CITYMumbler.Client.ViewModels
{
    public class CurrentChatUserListItemViewModel: ReactiveObject
    {
        private Client _client;
        private ushort _ownerId;

        public ushort ID => this._client.ID;

        public CurrentChatUserListItemViewModel(Client client, ushort ownedId)
        {
            this._client = client;
            this._ownerId = ownedId;
        }
    }
}
