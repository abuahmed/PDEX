using System.ComponentModel;
using System.Windows;
using PDEX.WPF.ViewModel;

namespace PDEX.WPF.Views
{
    /// <summary>
    /// Interaction logic for TenderLoans.xaml
    /// </summary>
    public partial class Tenders : Window
    {
        public Tenders()
        {
            InitializeComponent();
        }

        private void TenderLoans_OnClosing(object sender, CancelEventArgs e)
        {
            TenderViewModel.CleanUp();
        }
    }
}
