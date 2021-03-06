﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using ReactiveUI;
using Splat;

namespace CITYMumbler.Client.Views
{
    /// <summary>
    /// Interaction logic for SummaryView.xaml
    /// </summary>
    public partial class SummaryView : UserControl, IViewFor<SummaryViewModel>
    {
        public SummaryView()
        {
            InitializeComponent();
            this.ViewModel = Locator.Current.GetService<SummaryViewModel>();
            this.WhenActivated(d =>
            {
                d(this.OneWayBind(ViewModel, vm => vm.Groups, @this => @this.GroupsList.ItemsSource));
                d(this.OneWayBind(ViewModel, vm => vm.Users, @this => @this.UsersList.ItemsSource));
                
            });


        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (SummaryViewModel)value; }
        }
        public SummaryViewModel ViewModel { get; set; }
    }
}
