using System.Windows;
using System.Windows.Controls;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;
using PDEX.WPF.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace PDEX.WPF.Views
{
    /// <summary>
    /// Interaction logic for TenderLoanEntry.xaml
    /// </summary>
    public partial class TenderEntry : Window
    {
        public TenderEntry()
        {
            TenderEntryViewModel.Errors = 0;
            InitializeComponent();
        }
        
        public TenderEntry(TenderDTO tenderDTO)
        {
            TenderEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<TenderDTO>(tenderDTO);
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) TenderEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) TenderEntryViewModel.Errors -= 1;
        }

        private void WdwTenderLoanEntry_Loaded(object sender, RoutedEventArgs e)
        {
            TxtTenderNo.Focus();
        }
    }
}
