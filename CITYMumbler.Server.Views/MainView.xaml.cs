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
using CITYMumbler.Common.Contracts.Services.Logger;
using CITYMumbler.Server.ViewModels;
using ReactiveUI;

namespace CITYMumbler.Server.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl, IViewFor<MainViewModel>
    {
        private IDisposable sub;
        private Paragraph logParagraph;
        public MainView()
        {
            InitializeComponent();
            this.logParagraph = new Paragraph();
            this.LogOutput.Document = new FlowDocument(logParagraph);

            sub = this.WhenAnyObservable(x => x.ViewModel.Logs)
                .SubscribeOn(RxApp.MainThreadScheduler)
                .Subscribe(entry =>
                {
                    logParagraph.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        appendLogText(entry);
                        LogOutput.ScrollToEnd();
                    }));
                });

            this.Bind(ViewModel, vm => vm.Port, @this => @this.Port.Text);
            this.Bind(ViewModel, vm => vm.Threshold, @this => @this.Threshold.Text);
            this.OneWayBind(ViewModel, vm => vm.IsStartEnabled, @this => @this.StartButton.IsEnabled);
            this.OneWayBind(ViewModel, vm => vm.IsStopEnabled, @this => @this.StopButton.IsEnabled);


            this.BindCommand(ViewModel, vm => vm.StartCommand, @this => @this.StartButton);
            this.BindCommand(ViewModel, vm => vm.StopCommand, @this => @this.StopButton);

        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainViewModel) value; }
        }
        public MainViewModel ViewModel { get; set; }

        ~MainView()
        {
            this.sub.Dispose();
        }

        private void appendLogText(LogEntry entry)
        {
            var msg = string.Format("{0} {1} - {2}\r\n", entry.Level.ToString().ToUpper(), entry.Timestamp, entry.Message);
            Run run = new Run(msg)
            {
                Foreground = getColorForLevel(entry.Level)
            };
            logParagraph.Inlines.Add(run);
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

    }
}

