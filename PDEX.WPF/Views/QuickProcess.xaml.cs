using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;
using PDEX.WPF.ViewModel;

namespace PDEX.WPF.Views
{
    /// <summary>
    /// Interaction logic for QuickProcess.xaml
    /// </summary>
    public partial class QuickProcess : Window
    {
        public QuickProcess()
        {
            QuickProcessViewModel.Errors = 0;
            InitializeComponent();
        }

        public QuickProcess(ProcessTypes processType, int processId)
        {
            QuickProcessViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<int>(processId);
            Messenger.Default.Send<ProcessTypes>(processType);
            Messenger.Reset();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) QuickProcessViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) QuickProcessViewModel.Errors -= 1;
        }

        private void QuickProcess_OnClosing(object sender, CancelEventArgs e)
        {
            QuickProcessViewModel.CleanUp();
        }

        private void WdwQuickProcess_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        
        private void ChkStarted_OnChecked(object sender, RoutedEventArgs e)
        {
            if (ChkStarted.IsChecked != null && (bool) ChkStarted.IsChecked)
            {
                DtStartDate.SelectedValue = DateTime.Now;
                DtStartDate.IsEnabled = true;
            }
            else
            {
                DtStartDate.SelectedValue = null;
                DtStartDate.IsEnabled = false;
            }
        }

        private void ChkAccepted_OnChecked(object sender, RoutedEventArgs e)
        {
            if (ChkAccepted.IsChecked != null && (bool)ChkAccepted.IsChecked)
                DtEndDate.SelectedValue = DateTime.Now;
            else
                DtEndDate.SelectedValue = null;
        }

        //private void ChkDelStarted_OnChecked(object sender, RoutedEventArgs e)
        //{
        //    if (ChkDelStarted.IsChecked != null && (bool)ChkDelStarted.IsChecked)
        //        DtDelStartDate.SelectedValue = DateTime.Now;
        //    else
        //        DtDelStartDate.SelectedValue = null;
        //}

        //private void ChkDelivered_OnChecked(object sender, RoutedEventArgs e)
        //{
        //    if (ChkDelivered.IsChecked != null && (bool)ChkDelivered.IsChecked)
        //        DtDelEndDate.SelectedValue = DateTime.Now;
        //    else
        //        DtDelEndDate.SelectedValue = null;
        //}
    }
}
