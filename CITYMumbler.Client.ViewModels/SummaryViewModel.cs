using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
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

        public ReactiveList<Group> Groups;

        public SummaryViewModel()
        {
            this._client = Locator.Current.GetService<MumblerClient>();
            this.Groups = this._client.Groups;
            //this._client.Groups.ItemsAdded.Subscribe(g =>
            //{
            //    this.Groups.Add(g);
            //});
            //this._client.Groups.ShouldReset.Subscribe(_ =>
            //{
            //    this.Groups.Clear();
            //    foreach (var g in this._client.Groups)
            //    {
            //        this.Groups.Add(g);
            //    }
            //});
        }
    }
}
