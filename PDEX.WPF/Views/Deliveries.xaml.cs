using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PDEX.WPF.ViewModel;

namespace PDEX.WPF.Views
{
    /// <summary>
    /// Interaction logic for Deliveries.xaml
    /// </summary>
    public partial class Deliveries : UserControl
    {
        public Deliveries()
        {
            DeliveryViewModel.Errors = 0;
            DeliveryViewModel.LineErrors = 0;
            InitializeComponent();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) DeliveryViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) DeliveryViewModel.Errors -= 1;
        }
        private void Validation_LineError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) DeliveryViewModel.LineErrors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) DeliveryViewModel.LineErrors -= 1;
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //DeliveryViewModel.CleanUp();
        }
        private void BtnSaveLine_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //FocusControls();
        }
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            FocusControls();
        }
        private void BtnAddNewDelivery_Click(object sender, RoutedEventArgs e)
        {
            FocusControls();
        }
        private void FocusControls()
        {
            //LstItemsAutoCompleteBox.Focus();
        }
        private void LstDeliverys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FocusControls();
        }
        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            FocusControls();
        }
    }
}
