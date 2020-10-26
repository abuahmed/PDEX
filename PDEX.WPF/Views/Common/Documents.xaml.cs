using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PDEX.Core.Models;
using PDEX.WPF.ViewModel;

namespace PDEX.WPF.Views
{
    /// <summary>
    /// Interaction logic for Documents.xaml
    /// </summary>
    public partial class Documents : Window
    {
        public Documents()
        {
            DocumentViewModel.Errors = 0;
            InitializeComponent();
            TxtBankBranch.Focus();
        }

        public Documents(ClientDTO client)
        {
            DocumentViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<ClientDTO>(client);
            Messenger.Reset();
            TxtBankBranch.Focus();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) DocumentViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) DocumentViewModel.Errors -= 1;
        }

        private void Documents_OnClosing(object sender, CancelEventArgs e)
        {
            DocumentViewModel.CleanUp();
        }

        private void BtnAddNewBa_OnClick(object sender, RoutedEventArgs e)
        {
            TxtBankBranch.Focus();
        }
    }
}
