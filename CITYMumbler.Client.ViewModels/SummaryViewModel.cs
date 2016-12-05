using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Data;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Client.ViewModels
{
    public class SummaryViewModel: ReactiveObject
    {
        #region Private Members
        private readonly MumblerClient _client;
        #endregion

        //public IObservable<Client> Users;

        public IObservable<Group> Groups;

        public SummaryViewModel()
        {
            this._client = Locator.Current.GetService<MumblerClient>();
            //this.Users = this._client.
        }
    }
}
