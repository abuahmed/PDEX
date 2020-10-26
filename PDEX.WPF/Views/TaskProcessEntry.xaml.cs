using System.ComponentModel;
using PDEX.Core.Enumerations;
using PDEX.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PDEX.WPF.Views
{
    /// <summary>
    /// Interaction logic for Processs.xaml
    /// </summary>
    public partial class TaskProcessEntry : Window
    {
        public TaskProcessEntry()
        {
            TaskProcessViewModel.Errors = 0;
            InitializeComponent();
        }
        public TaskProcessEntry(TaskProcessTypes businessPartnerType)
        {
            TaskProcessEntryViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<TaskProcessTypes>(businessPartnerType);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) TaskProcessEntryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) TaskProcessEntryViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CustName.Focus();
        }

        private void Processs_OnClosing(object sender, CancelEventArgs e)
        {
            TaskProcessViewModel.CleanUp();
        }
    }
}
