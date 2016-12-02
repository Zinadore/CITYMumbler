using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CITYMumbler.Common.Contracts.Services.Logger;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Client.ViewModels
{
    public class LogWindowViewModel: ReactiveObject
    {
        public IObservable<LogEntry> Logs { get; private set; }
        public LogWindowViewModel() { this.Logs = Locator.Current.GetService<ILoggerService>().Entries; }
    }
}
