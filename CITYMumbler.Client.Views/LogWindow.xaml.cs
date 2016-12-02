using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CITYMumbler.Client.ViewModels;
using CITYMumbler.Common.Contracts.Services.Logger;
using ReactiveUI;
using Splat;
using LogLevel = CITYMumbler.Common.Contracts.Services.Logger.LogLevel;

namespace CITYMumbler.Client.Views
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : UserControl, IViewFor<LogWindowViewModel>
    {
        #region Private Members

        private Paragraph _logParagraph;
        #endregion
        public LogWindow()
        {
            InitializeComponent();
            this.ViewModel = Locator.Current.GetService<LogWindowViewModel>();

            this._logParagraph = new Paragraph();
            this.LogOutput.Document = new FlowDocument(this._logParagraph);

            this.WhenAnyObservable(x => x.ViewModel.Logs)
                .SubscribeOn(RxApp.TaskpoolScheduler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(entry =>
                {
                    appendLogText(entry);
                    LogOutput.ScrollToEnd();
                });

        }

        private void appendLogText(LogEntry entry)
        {
            var msg = string.Format("{0} {1} - {2}\r\n", entry.Level.ToString().ToUpper(), entry.Timestamp, entry.Message);
            Run run = new Run(msg)
            {
                Foreground = getColorForLevel(entry.Level)
            };
            this._logParagraph.Inlines.Add(run);
        }

        private Brush getColorForLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return Brushes.Chocolate;
                case LogLevel.Info:
                    return Brushes.CornflowerBlue;
                case LogLevel.Warn:
                    return Brushes.Orange;
                case LogLevel.Error:
                    return Brushes.Red;
                default:
                    return Brushes.AliceBlue;
            }

        }
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (LogWindowViewModel)value; }
        }
        public LogWindowViewModel ViewModel { get; set; }
    }
}
