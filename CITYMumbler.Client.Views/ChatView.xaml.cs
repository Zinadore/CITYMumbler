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
using CITYMumbler.Common.Utilities;
using ReactiveUI;
using Splat;

namespace CITYMumbler.Client.Views
{
	/// <summary>
	/// Interaction logic for ChatView.xaml
	/// </summary>
	public partial class ChatView : UserControl, IViewFor<ChatViewModel>
	{
	    private Paragraph chatParagraph;
		public ChatView()
		{
			InitializeComponent();
            chatParagraph = new Paragraph();
            this.ChatDisplay.Document = new FlowDocument(chatParagraph);
			this.Bind(this.ViewModel, vm => vm.ChatInput, @this => @this.ChatInput.Text);

			this.BindCommand(ViewModel, vm => vm.SendCommand, @this => @this.SendButton);
		    this.WhenActivated(d =>
		    {
		        d(this.WhenAnyObservable(x => x.ViewModel.Entries)
                    .SubscribeOn(RxApp.TaskpoolScheduler)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(entry =>
                    {
                        addChatEntry(entry);
                        this.ChatDisplay.ScrollToEnd();
                    }));
		        d(this.ChatInput.Events()
		              .KeyDown
		              .Where(x => x.Key == Key.Enter)
		              .Subscribe(_ =>
		              {
		                  this.ViewModel.SendCommand.Execute();
		              }));

		    });


		}

	    private void addChatEntry(ChatEntry entry)
	    {
	        var run = new Run(entry.SenderName)
	        {
	            Foreground = getColorForSender(entry.SenderId),
                FontWeight = getFontWeightForSender(entry.SenderId)
	        };
            this.chatParagraph.Inlines.Add(run);

	        var msg = string.Format(": {0}\r\n", entry.Message);
	        run = new Run(msg);
	        this.chatParagraph.Inlines.Add(run);
	    }

	    private FontWeight getFontWeightForSender(ushort senderId)
	    {
	        return senderId == this.ViewModel.ID ? FontWeights.Bold : FontWeights.Normal;
	    }

	    private Brush getColorForSender(ushort senderId)
	    {
	        return senderId == this.ViewModel.ID ? new SolidColorBrush(AccentColorSet.ActiveSet["SystemAccent"]) : Brushes.Black;
	    }

	    object IViewFor.ViewModel
		{
			get { return ViewModel; }
			set { ViewModel = (ChatViewModel) value; }
		}
		public ChatViewModel ViewModel { get; set; }

        private void View_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, new Action(() =>
            {
                this.ChatInput.Focus();
                Keyboard.Focus(this.ChatInput);
            }));
        }
    }
}
