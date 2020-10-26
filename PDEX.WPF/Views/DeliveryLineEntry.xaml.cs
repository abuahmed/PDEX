using System;
using System.ComponentModel;
using System.Windows.Media;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;
using PDEX.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PDEX.WPF.Views
{
    /// <summary>
    /// Interaction logic for DeliveryLineEntry.xaml
    /// </summary>
    public partial class DeliveryLineEntry : Window
    {
        public DeliveryLineEntry()
        {
            DeliveryLineViewModel.Errors = 0;
            InitializeComponent();
        }

        public DeliveryLineEntry(DeliveryHeaderDTO deliveryDTO)
        {
            DeliveryLineViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<DeliveryHeaderDTO>(deliveryDTO);
            Messenger.Reset();
        }

        public DeliveryLineEntry(DeliveryLineDTO deliveryLineDTO)
        {
            DeliveryLineViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<DeliveryLineDTO>(deliveryLineDTO);
            Messenger.Reset();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) DeliveryLineViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) DeliveryLineViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LstDeliveryLinesAutoCompleteBox.Focus();
        }

        private void DeliveryLineEntry_OnClosing(object sender, CancelEventArgs e)
        {
            //DeliveryLineViewModel.CleanUp();
        }

        private void LstDeliveryLinesAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void LstDeliveryLinesFromPersonAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
