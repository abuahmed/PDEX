using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PDEX.Core.Models;
using PDEX.WPF.ViewModel;

namespace PDEX.WPF.Views
{
    /// <summary>
    /// Interaction logic for AcceptanceEntry.xaml
    /// </summary>
    public partial class AcceptanceEntry : Window
    {
        public AcceptanceEntry()
        {
            AcceptanceEntryViewModel.Errors = 0;
            InitializeComponent();
        }
        public AcceptanceEntry(DeliveryHeaderDTO deliveryDTO)
        {
            AcceptanceEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<DeliveryHeaderDTO>(deliveryDTO);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) AcceptanceEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) AcceptanceEntryViewModel.Errors -= 1;
        }

        private void AcceptanceEntry_OnClosing(object sender, CancelEventArgs e)
        {
            AcceptanceEntryViewModel.CleanUp();
        }

        private void WdwAcceptanceEntry_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ChkStarted_OnChecked(object sender, RoutedEventArgs e)
        {
            if(ChkStarted.IsChecked != null && (bool) ChkStarted.IsChecked)
                DtStartDate.SelectedValue=DateTime.Now;
            else
                DtStartDate.SelectedValue = null;
        }

        private void ChkAccepted_OnChecked(object sender, RoutedEventArgs e)
        {
            if (ChkAccepted.IsChecked != null && (bool)ChkAccepted.IsChecked)
                DtEndDate.SelectedValue = DateTime.Now;
            else
                DtEndDate.SelectedValue = null;
        }
    }
}
