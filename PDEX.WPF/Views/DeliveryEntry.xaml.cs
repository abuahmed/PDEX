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
    /// Interaction logic for DeliveryEntry.xaml
    /// </summary>
    public partial class DeliveryEntry : Window
    {
        public DeliveryEntry()
        {
            DeliveryEntryViewModel.Errors = 0;
            InitializeComponent();
        }
        public DeliveryEntry(DeliveryLineDTO deliveryLineDTO)
        {
            DeliveryEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<DeliveryLineDTO>(deliveryLineDTO);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) DeliveryEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) DeliveryEntryViewModel.Errors -= 1;
        }

        private void DeliveryEntry_OnClosing(object sender, CancelEventArgs e)
        {
            DeliveryEntryViewModel.CleanUp();
        }

        private void WdwDeliveryEntry_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ChkStarted_OnChecked(object sender, RoutedEventArgs e)
        {
            if (ChkStarted.IsChecked != null && (bool)ChkStarted.IsChecked)
                DtStartDate.SelectedValue = DateTime.Now;
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
