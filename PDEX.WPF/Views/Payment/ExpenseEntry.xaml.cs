using System.Windows;
using System.Windows.Controls;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;
using PDEX.WPF.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace PDEX.WPF.Views
{
    /// <summary>
    /// Interaction logic for ExpenseLoanEntry.xaml
    /// </summary>
    public partial class ExpenseEntry : Window
    {
        public ExpenseEntry()
        {
            ExpenseEntryViewModel.Errors = 0;
            InitializeComponent();
        }
        public ExpenseEntry(PaymentTypes paymentType)
        {
            ExpenseEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<PaymentTypes>(paymentType);
        }
        public ExpenseEntry(PaymentDTO paymentDTO)
        {
            ExpenseEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<PaymentDTO>(paymentDTO);
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ExpenseEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ExpenseEntryViewModel.Errors -= 1;
        }

        private void WdwExpenseLoanEntry_Loaded(object sender, RoutedEventArgs e)
        {
            TxtReason.Focus();
        }
    }
}
