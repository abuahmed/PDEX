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
    public class OrderByClientViewModel : ViewModelBase
    {
        #region Fields
        private static IClientService _clientService;
        private static IDeliveryService _deliveryService;
        private ClientDTO _selectedOrderByClient;
        private DeliveryHeaderDTO _delivery, _deliveryParam;
        private IEnumerable<ClientDTO> _sendersList;
        private ObservableCollection<ClientDTO> _senders;
        private ICommand _saveOrderByClientViewCommand;
        #endregion

        #region Constructor
        public OrderByClientViewModel()
        {
            CleanUp();
            _clientService = new ClientService();
            _deliveryService = new DeliveryService();

            GetLiveOrderByClients();
            CheckRoles();

            Messenger.Default.Register<DeliveryHeaderDTO>(this, (message) =>
            {
                DeliveryParam = message;
            });
        }
        public static void CleanUp()
        {
            if (_clientService != null)
                _clientService.Dispose();
            if (_deliveryService != null)
                _deliveryService.Dispose();
        }
        #endregion

        #region Public Properties
        public DeliveryHeaderDTO DeliveryParam
        {
            get { return _deliveryParam; }
            set
            {
                _deliveryParam = value;
                RaisePropertyChanged<DeliveryHeaderDTO>(() => DeliveryParam);
                if (DeliveryParam != null)
                {
                    var criteria = new SearchCriteria<DeliveryHeaderDTO>
                    {
                        CurrentUserId = Singleton.User.UserId
                    };
                    criteria.FiList.Add(d => d.Id == DeliveryParam.Id);

                    int totCount;
                    Delivery = _deliveryService.GetAll(criteria, out totCount).FirstOrDefault();
                }
            }
        }
        public DeliveryHeaderDTO Delivery
        {
            get { return _delivery; }
            set
            {
                _delivery = value;
                RaisePropertyChanged<DeliveryHeaderDTO>(() => Delivery);
                if (Delivery != null && Delivery.OrderByClient != null)
                    SelectedOrderByClient = Delivery.OrderByClient;
            }
        }

        public ClientDTO SelectedOrderByClient
        {
            get { return _selectedOrderByClient; }
            set
            {
                _selectedOrderByClient = value;
                RaisePropertyChanged<ClientDTO>(() => SelectedOrderByClient);
            }
        }
        public IEnumerable<ClientDTO> OrderByClientsList
        {
            get { return _sendersList; }
            set
            {
                _sendersList = value;
                RaisePropertyChanged<IEnumerable<ClientDTO>>(() => OrderByClientsList);
            }
        }
        public ObservableCollection<ClientDTO> OrderByClients
        {
            get { return _senders; }
            set
            {
                _senders = value;
                RaisePropertyChanged<ObservableCollection<ClientDTO>>(() => OrderByClients);
            }
        }

        #endregion

        #region Commands
        public ICommand SaveOrderByClientViewCommand
        {
            get { return _saveOrderByClientViewCommand ?? (_saveOrderByClientViewCommand = new RelayCommand<Object>(SaveOrderByClient, CanSave)); }
        }

        private void SaveOrderByClient(object obj)
        {
            try
            {
                Delivery.OrderByClient = null;
                Delivery.OrderByClientId = SelectedOrderByClient.Id;
                _deliveryService.InsertOrUpdate(Delivery);

                DeliveryParam.OrderByClientId= SelectedOrderByClient.Id;//Delete if duplicate Client Data found

                CloseWindow(obj);
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

        public void GetLiveOrderByClients()
        {
            var criteria = new SearchCriteria<ClientDTO>();

            criteria.FiList.Add(b => !b.IsReceiver && b.IsActive);

            OrderByClientsList = _clientService.GetAll(criteria)
               .OrderBy(i => i.Id)
               .ToList();

            OrderByClients = new ObservableCollection<ClientDTO>(OrderByClientsList);
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
