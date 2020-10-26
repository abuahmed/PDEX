using System.ComponentModel;
using System.Windows;
using PDEX.WPF.ViewModel;

namespace PDEX.WPF.Views
{
    /// <summary>
    /// Interaction logic for ExpenseLoans.xaml
    /// </summary>
    public partial class Expenses : Window
    {
        public Expenses()
        {
            InitializeComponent();
        }

        private void ExpenseLoans_OnClosing(object sender, CancelEventArgs e)
        {
            ExpenseViewModel.CleanUp();
        }
    }
}
