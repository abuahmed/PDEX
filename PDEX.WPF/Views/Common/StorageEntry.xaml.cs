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
    /// Interaction logic for StorageEntry.xaml
    /// </summary>
    public partial class StorageEntry : Window
    {
        public StorageEntry()
        {
            StorageBinViewModel.Errors = 0;
            InitializeComponent();
        }

        //public StorageEntry(DeliveryHeaderDTO deliveryDTO)
        //{
        //    StorageBinViewModel.Errors = 0;
        //    InitializeComponent();
        //    Messenger.Default.Send<DeliveryHeaderDTO>(deliveryDTO);
        //    Messenger.Reset();
        //}

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) StorageBinViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) StorageBinViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LstStoragesAutoCompleteBox.Focus();
        }

        private void StorageEntry_OnClosing(object sender, CancelEventArgs e)
        {
            //StorageViewModel.CleanUp();
        }

        private void LstStoragesAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
