using System;
using PDEX.Core;
using PDEX.Core.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PDEX.WPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _headerText, _titleText;
        readonly static DeliveryViewModel DeliveryViewModel = new ViewModelLocator().Delivery;
        readonly static FollowUpViewModel FollowUpViewModel = new ViewModelLocator().FollowUp;
        private ViewModelBase _currentViewModel;

        public MainViewModel()
        {
            CheckRoles();
            TitleText = "PDMAS V1.0.0.0, PDEX Delivery Management System - " +
                        Singleton.User.UserName + " - " +
                        DateTime.Now.ToString("dd/MM/yyyy") + " - " +
                        ReportUtility.GetEthCalendarFormated(DateTime.Now, "/");

            HeaderText = "Request Managment";
            DeliveryViewModel.LoadData = true;
            CurrentViewModel = DeliveryViewModel;

            DeliveryViewModelViewCommand = new RelayCommand(ExecuteDeliveryViewModelViewCommand);
            FollowUpViewModelViewCommand = new RelayCommand(ExecuteFollowUpViewModelViewCommand);
        }

        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                if (_currentViewModel == value)
                    return;
                _currentViewModel = value;
                RaisePropertyChanged("CurrentViewModel");
            }
        }
        
        public RelayCommand DeliveryViewModelViewCommand { get; private set; }
        private void ExecuteDeliveryViewModelViewCommand()
        {
            HeaderText = "Request Managment";
            DeliveryViewModel.LoadData = true;
            CurrentViewModel = DeliveryViewModel;
        }

        public RelayCommand FollowUpViewModelViewCommand { get; private set; }
        private void ExecuteFollowUpViewModelViewCommand()
        {
            HeaderText = "Followup Managment";
            FollowUpViewModel.LoadData = true;
            CurrentViewModel = FollowUpViewModel;
        }
        
        public string HeaderText
        {
            get
            {
                return _headerText;
            }
            set
            {
                if (_headerText == value)
                    return;
                _headerText = value;
                RaisePropertyChanged("HeaderText");
            }
        }

        public string TitleText
        {
            get
            {
                return _titleText;
            }
            set
            {
                if (_titleText == value)
                    return;
                _titleText = value;
                RaisePropertyChanged("TitleText");
            }
        }

        #region Previlege Visibility
        private UserRolesModel _userRoles;

        public UserRolesModel UserRoles
        {
            get { return _userRoles; }
            set
            {
                _userRoles = value;
                RaisePropertyChanged<UserRolesModel>(() => UserRoles);
            }
        }

        private void CheckRoles()
        {
            UserRoles = Singleton.UserRoles;
            UserRoles.Admin = UserRoles.Settings == "Visible" || 
                                UserRoles.AdvancedSettings == "Visible" ||
                                UserRoles.UsersMgmt == "Visible" ||
                                UserRoles.BackupRestore == "Visible" 
                            ? "Visible" : "Collapsed";

            UserRoles.Settings = UserRoles.AdvancedSettings == "Visible"
                ? "Visible"
                : "Collapsed";
  
        }

        #endregion
    }
}