using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Client.ViewModels
{
    public class SummaryViewModel: ReactiveObject
    {
        #region Private Members
        private readonly MumblerClient _client;
        #endregion

        public SummaryViewModel()
        {
            this._client = Locator.Current.GetService<MumblerClient>();
        }
    }
}
