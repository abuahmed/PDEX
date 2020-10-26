using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using PDEX.Core;
using PDEX.Core.Common;
using PDEX.Core.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PDEX.Service;
using PDEX.Service.Interfaces;

namespace PDEX.WPF.ViewModel
{
    public class StorageBinViewModel : ViewModelBase
    {
        #region Fields
        private static IStorageBinService _storageBinService;
        private static IDeliveryService _messageService;
        private StorageBinDTO _selectedStorageBin;
        private MessageDTO _selectedMessage;
        private IEnumerable<StorageBinDTO> _storageBinsList;
        private ObservableCollection<StorageBinDTO> _storageBins;
        private ObservableCollection<MessageDTO> _messages;
        private ICommand _saveStorageBinViewCommand;
        #endregion

        #region Constructor
        public StorageBinViewModel()
        {
            CleanUp();
            _storageBinService = new StorageBinService();
            _messageService = new DeliveryService();

            GetLiveStorageBins();
            GetLiveMessages();
            CheckRoles();
        }
        public static void CleanUp()
        {
            if (_storageBinService != null)
                _storageBinService.Dispose();
            if (_messageService != null)
                _messageService.Dispose();
        }
        #endregion

        #region Public Properties
        
        public MessageDTO SelectedMessage
        {
            get { return _selectedMessage; }
            set
            {
                _selectedMessage = value;
                RaisePropertyChanged<MessageDTO>(() => SelectedMessage);
                if (SelectedMessage != null)
                {
                    if (SelectedMessage.StorageBinId != null)
                    {
                        SelectedStorageBin = StorageBins.FirstOrDefault(s => s.Id == SelectedMessage.StorageBinId);
                    }
                }
            }
        }
        public ObservableCollection<MessageDTO> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                RaisePropertyChanged<ObservableCollection<MessageDTO>>(() => Messages);
            }
        }

        public StorageBinDTO SelectedStorageBin
        {
            get { return _selectedStorageBin; }
            set
            {
                _selectedStorageBin = value;
                RaisePropertyChanged<StorageBinDTO>(() => SelectedStorageBin);
            }
        }
        public IEnumerable<StorageBinDTO> StorageBinsList
        {
            get { return _storageBinsList; }
            set
            {
                _storageBinsList = value;
                RaisePropertyChanged<IEnumerable<StorageBinDTO>>(() => StorageBinsList);
            }
        }
        public ObservableCollection<StorageBinDTO> StorageBins
        {
            get { return _storageBins; }
            set
            {
                _storageBins = value;
                RaisePropertyChanged<ObservableCollection<StorageBinDTO>>(() => StorageBins);
            }
        }

        #endregion

        #region Commands
        public ICommand SaveStorageBinViewCommand
        {
            get { return _saveStorageBinViewCommand ?? (_saveStorageBinViewCommand = new RelayCommand<Object>(SaveStorageBin, CanSave)); }
        }

        private void SaveStorageBin(object obj)
        {
            try
            {
                if (SelectedStorageBin != null && SelectedStorageBin.Id != 0)
                {
                    SelectedMessage.StorageBinId = SelectedStorageBin.Id;
                    _messageService.InsertOrUpdateMessageChild(SelectedMessage);
                }

                //CloseWindow(obj);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                  + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public void CloseWindow(object obj)
        {
            if (obj != null)
            {
                var window = obj as Window;
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
        }
        #endregion

        public void GetLiveMessages()
        {
            var criteria = new SearchCriteria<MessageDTO>();

            int totalCount;
            var messagesList = _messageService.GetAllMessageChilds(criteria, out totalCount)
               .OrderBy(i => i.Id)
               .ToList();

            Messages = new ObservableCollection<MessageDTO>(messagesList);
        }

        public void GetLiveStorageBins()
        {
            var criteria = new SearchCriteria<StorageBinDTO>();

            criteria.FiList.Add(b => b.IsActive);

            StorageBinsList = _storageBinService.GetAll(criteria)
               .OrderBy(i => i.Id)
               .ToList();

            StorageBins = new ObservableCollection<StorageBinDTO>(StorageBinsList);
        }

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object parameter)
        {
            return Errors == 0;
        }
        #endregion

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
        }

        #endregion
    }
}
