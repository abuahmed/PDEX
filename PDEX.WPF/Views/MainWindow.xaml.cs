using System.Windows;
using PDEX.Core.Enumerations;

namespace PDEX.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            new ChangePassword().ShowDialog();
        }

        private void UsersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Users().Show();
        }

        private void BackupRestoreMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new BackupRestore().ShowDialog();
        }

        private void Clients_Click(object sender, RoutedEventArgs e)
        {
            new Clients().Show();
        }

        private void Staffs_Click(object sender, RoutedEventArgs e)
        {
            new Staffs().Show();
        }

        private void Vehicles_Click(object sender, RoutedEventArgs e)
        {
            new Vehicles().Show();
        }

        private void CompanyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Company().Show();
        }

        private void StoresMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Warehouses().Show();
        }

        private void StoreMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            new StorageEntry().Show();
        }

        private void ProcessMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            new TaskProcesses().Show();
        }

        private void ExpenseCashLoanListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Expenses().Show();
        }

        private void TenderMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            new Tenders().Show();
        }
    }
}
