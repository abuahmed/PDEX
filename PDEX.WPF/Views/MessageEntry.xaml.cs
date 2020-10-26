using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using PDEX.Core.Models;
using PDEX.WPF.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace PDEX.WPF.Views
{
    /// <summary>
    /// Interaction logic for Item.xaml
    /// </summary>
    public partial class MessageEntry : Window
    {
        public MessageEntry()
        {
            MessageViewModel.Errors = 0;
            InitializeComponent();
            TxtItemName.Focus();
        }

        public MessageEntry(MessageDTO messageDto)
        {
            MessageViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<MessageDTO>(messageDto);
            Messenger.Reset();
        }
  
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) MessageViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) MessageViewModel.Errors -= 1;
        }

        private void WdwMessage_Loaded(object sender, RoutedEventArgs e)
        {
            TxtItemName.Focus();
        }

        private void Message_OnClosing(object sender, CancelEventArgs e)
        {
            MessageViewModel.CleanUp();
        }
    }
}
