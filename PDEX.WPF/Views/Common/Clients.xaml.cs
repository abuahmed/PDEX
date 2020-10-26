using System.ComponentModel;
using PDEX.Core.Enumerations;
using PDEX.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PDEX.WPF.Views
{
    /// <summary>
    /// Interaction logic for Clients.xaml
    /// </summary>
    public partial class Clients : Window
    {
        public Clients()
        {
            ClientViewModel.Errors = 0;
            InitializeComponent();
        }
        public Clients(ClientTypes businessPartnerType)
        {
            ClientViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<ClientTypes>(businessPartnerType);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ClientViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ClientViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CustName.Focus();
        }

        private void Clients_OnClosing(object sender, CancelEventArgs e)
        {
            ClientViewModel.CleanUp();
        }
    }
}
