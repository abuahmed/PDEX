using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PDEX.DAL;
using PDEX.Repository;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PDEX.Core.Models;
using PDEX.Repository.Interfaces;
using PDEX.WPF.Views;

namespace PDEX.WPF.ViewModel
{
    public class SplashScreenViewModel : ViewModelBase
    {
        #region Fields
        private static IUnitOfWork _unitOfWork;
        private object _splashWindow;
        bool _login;
        private string _licensedTo;
        #endregion

        #region Constructor
        public SplashScreenViewModel()
        {
            CleanUp();
            _unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());

            Messenger.Default.Register<object>(this, (message) =>
            {
                SplashWindow = message;
            });
        }
        public static void CleanUp()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }
        #endregion

        #region Properties

        public object SplashWindow
        {
            get { return _splashWindow; }
            set
            {
                _splashWindow = value;
                RaisePropertyChanged<object>(() => SplashWindow);
                if (SplashWindow != null)
                    CheckActivation();
            }
        }
        public string LicensedTo
        {
            get { return _licensedTo; }
            set
            {
                _licensedTo = value;
                RaisePropertyChanged<string>(() => LicensedTo);
            }
        }
        #endregion

        #region Actions
        private void CheckActivation()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var activation = _unitOfWork.Repository<CompanyDTO>().Query().Get().FirstOrDefault();
                if (activation == null)
                {
                    _unitOfWork.Repository<CompanyDTO>().Insert(new CompanyDTO
                    {
                        Id = 1,
                        DisplayName = "PDEX Post Service PLC",
                        Address = new AddressDTO
                        {
                            AddressDetail = "Piasa, Habtegiorgis, Tefera Seyioum Business Center, 11th Floor Office No. 1105",
                            Country = "Ethiopia",
                            City = "Addis Abeba",
                            Telephone = "0111266701",
                            Mobile = "0929142121",
                            AlternateMobile = "0911168312",
                            PrimaryEmail = "pdexservice@gmail.com"
                        }
                    });

                    _unitOfWork.Repository<WarehouseDTO>().Insert(new WarehouseDTO
                    {
                        Id = 1,
                        DisplayName = "Main Store",
                        Address = new AddressDTO
                        {
                            AddressDetail = "Piasa, Habtegiorgis, Tefera Seyioum Business Center, 11th Floor Office No. 1105",
                            Country = "Ethiopia",
                            City = "Addis Abeba",
                            Telephone = "0111266701",
                            Mobile = "0929142121",
                            AlternateMobile = "0911168312",
                            PrimaryEmail = "pdexservice@gmail.com"
                        }
                    });

                    _unitOfWork.Commit();
                }
                _login = true;

            }
            catch
            {
                MessageBox.Show("Problem opening amstock, may be the server computer or the network not working properly! try again later..",
                    "Error Opening", MessageBoxButton.OK, MessageBoxImage.Error);
                CloseWindow(SplashWindow);
            }
        }
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (_login)
            {
                if (_unitOfWork.Repository<CompanyDTO>().Query().Get().FirstOrDefault() == null)
                {
                    MessageBox.Show("The server is not yet ready for work, contact your administrator...");
                    CloseWindow(SplashWindow);
                }
                new Login().Show();
            }

            CloseWindow(SplashWindow);
        }

        private ICommand _closeWindowCommand;
        public ICommand CloseWindowCommand
        {
            get
            {
                return _closeWindowCommand ?? (_closeWindowCommand = new RelayCommand<Object>(CloseWindow));
            }
        }

        public void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window == null) return;
            window.Close();
        }
        #endregion
    }
}
