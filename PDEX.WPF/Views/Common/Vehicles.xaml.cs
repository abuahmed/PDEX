using System.ComponentModel;
using PDEX.Core.Enumerations;
using PDEX.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace PDEX.WPF.Views
{
    /// <summary>
    /// Interaction logic for Vehicles.xaml
    /// </summary>
    public partial class Vehicles : Window
    {
        public Vehicles()
        {
            VehicleViewModel.Errors = 0;
            InitializeComponent();
        }
        public Vehicles(VehicleTypes businessPartnerType)
        {
            VehicleViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<VehicleTypes>(businessPartnerType);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) VehicleViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) VehicleViewModel.Errors -= 1;
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TxtPlateNumber.Focus();
        }

        private void Vehicles_OnClosing(object sender, CancelEventArgs e)
        {
            VehicleViewModel.CleanUp();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            TxtPlateNumber.Focus();
        }
    }
}
