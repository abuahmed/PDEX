using System;
using System.ComponentModel;
using System.Windows.Media;
using PDEX.Core.Enumerations;
using PDEX.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PDEX.WPF.Views
{
    /// <summary>
    /// Interaction logic for Staffs.xaml
    /// </summary>
    public partial class Staffs : Window
    {
        public Staffs()
        {
            StaffViewModel.Errors = 0;
            InitializeComponent();
        }
        public Staffs(StaffTypes businessPartnerType)
        {
            StaffViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<StaffTypes>(businessPartnerType);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) StaffViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) StaffViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void dtBirthDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (DtBirthDate.SelectedDate == null) return;
                int age = DateTime.Now.Subtract(DtBirthDate.SelectedDate.Value).Days;
                age = (int) (age / 365.25);
                //try
                //{
                //    LblAge.Text = age.ToString().Substring(0, 4);
                //}
                //catch
                //{
                    LblAge.Text = age.ToString();
                //}
                //LblAge.Foreground = Brushes.Black;
            }
            catch
            {

            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TXtCustName.Focus();
        }

        private void Staffs_OnClosing(object sender, CancelEventArgs e)
        {
            StaffViewModel.CleanUp();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            LblAge.Text = "";
            TXtCustName.Focus();
        }
    }
}
