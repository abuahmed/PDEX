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
    /// Interaction logic for OrderByClientEntry.xaml
    /// </summary>
    public partial class OrderByClientEntry : Window
    {
        public OrderByClientEntry()
        {
            OrderByClientViewModel.Errors = 0;
            InitializeComponent();
        }

        public OrderByClientEntry(DeliveryHeaderDTO deliveryDTO)
        {
            OrderByClientViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<DeliveryHeaderDTO>(deliveryDTO);
            Messenger.Reset();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) OrderByClientViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) OrderByClientViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LstOrderByClientsAutoCompleteBox.Focus();
        }

        private void OrderByClientEntry_OnClosing(object sender, CancelEventArgs e)
        {
            OrderByClientViewModel.CleanUp();
        }

        private void LstOrderByClientsAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
