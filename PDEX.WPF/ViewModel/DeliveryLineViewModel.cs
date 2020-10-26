using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using PDEX.Core;
using PDEX.Core.Common;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PDEX.Service;
using PDEX.Service.Interfaces;
using PDEX.WPF.Views;

namespace PDEX.WPF.ViewModel
{
    public class DeliveryLineViewModel : ViewModelBase
    {
        #region Fields
        private static IClientService _clientService;
        private static IDeliveryService _deliveryService;
        private DeliveryHeaderDTO _delivery, _deliveryParam;
        private DeliveryLineDTO _selectedDeliveryLine, _selectedDeliveryLineParam;
        private string _receiverName, _senderName;
        private ICommand _saveDeliveryLineViewCommand, _toAddressViewCommand, _fromAddressViewCommand;
        private ObservableCollection<AddressDTO> _fromAddressDetail, _toAddressDetail;
        #endregion

        #region Constructor
        public DeliveryLineViewModel()
        {
            CleanUp();
            _clientService = new ClientService();
            _deliveryService = new DeliveryService();

            GetReceivers();
            CheckRoles();

            Messenger.Default.Register<DeliveryHeaderDTO>(this, (message) =>
            {
                DeliveryParam = message;
            });

            Messenger.Default.Register<DeliveryLineDTO>(this, (message) =>
            {
                SelectedDeliveryLineParam = message;
            });

        }
        public static void CleanUp()
        {
            if (_clientService != null)
                _clientService.Dispose();
            if (_deliveryService != null)
                _deliveryService.Dispose();
        }

        private bool _isNew;
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
                    criteria.FiList.Add(dh => dh.Id == DeliveryParam.Id);
                    Delivery = _deliveryService.GetAll(criteria).FirstOrDefault();
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
                _isNew = true;

                if (Delivery != null)
                {
                    SelectedDeliveryLine = new DeliveryLineDTO
                    {
                        DeliveryType = DeliveryLineRouteTypes.Delivering,
                        Number = CommonUtility.GetSecretCode(),
                        UrgencyInHours = 24,
                        ReceiverSecretCode = CommonUtility.GetSecretCode(),
                        DeliveryHeaderId = Delivery.Id
                    };
                    SelectedSender = Receivers.FirstOrDefault(s => s.Id == Delivery.OrderByClientId);

                    FromAdressDetail = new ObservableCollection<AddressDTO>();
                    if (Delivery.OrderByClient != null && Delivery.OrderByClient.Address != null)
                    {
                        SelectedDeliveryLine.FromAddress = Delivery.OrderByClient.Address;
                        FromAdressDetail.Add(SelectedDeliveryLine.FromAddress);
                    }
                }
            }
        }

        public string ReceiverName
        {
            get { return _receiverName; }
            set
            {
                _receiverName = value;
                RaisePropertyChanged<string>(() => ReceiverName);
            }
        }
        public string SenderName
        {
            get { return _senderName; }
            set
            {
                _senderName = value;
                RaisePropertyChanged<string>(() => SenderName);
            }
        }

        public DeliveryLineDTO SelectedDeliveryLineParam
        {
            get { return _selectedDeliveryLineParam; }
            set
            {
                _selectedDeliveryLineParam = value;
                RaisePropertyChanged<DeliveryLineDTO>(() => SelectedDeliveryLineParam);
                if (SelectedDeliveryLineParam != null)
                {
                    var criteria = new SearchCriteria<DeliveryLineDTO>
                    {
                        CurrentUserId = Singleton.User.UserId
                    };
                    criteria.FiList.Add(dh => dh.Id == SelectedDeliveryLineParam.Id);

                    int totCount;
                    SelectedDeliveryLine = _deliveryService.GetAllChilds(criteria, out totCount).FirstOrDefault();
                }
            }
        }
        public DeliveryLineDTO SelectedDeliveryLine
        {
            get { return _selectedDeliveryLine; }
            set
            {
                _selectedDeliveryLine = value;
                RaisePropertyChanged<DeliveryLineDTO>(() => SelectedDeliveryLine);

                if (SelectedDeliveryLine != null)
                {
                    ToAdressDetail = new ObservableCollection<AddressDTO> { SelectedDeliveryLine.ToAddress };
                    SelectedReceiver = SelectedDeliveryLine.ToClient;
                    if (!_isNew)
                    {
                        FromAdressDetail = new ObservableCollection<AddressDTO> { SelectedDeliveryLine.FromAddress };
                        SelectedSender = SelectedDeliveryLine.FromClient;//Receivers.FirstOrDefault(s => s.Id == SelectedDeliveryLine.FromClientId);
                    }
                }

            }
        }

        public ObservableCollection<AddressDTO> FromAdressDetail
        {
            get { return _fromAddressDetail; }
            set
            {
                _fromAddressDetail = value;
                RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => FromAdressDetail);
            }
        }
        public ObservableCollection<AddressDTO> ToAdressDetail
        {
            get { return _toAddressDetail; }
            set
            {
                _toAddressDetail = value;
                RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => ToAdressDetail);
            }
        }
        #endregion

        #region Commands
        public ICommand SaveDeliveryLineViewCommand
        {
            get { return _saveDeliveryLineViewCommand ?? (_saveDeliveryLineViewCommand = new RelayCommand<Object>(SaveDeliveryLine, CanSave)); }
        }
        private void SaveDeliveryLine(object obj)
        {
            try
            {
                if (Delivery != null && (Delivery.CountLinesAll == 0 && _isNew))
                {
                    var selectedDeliveryLineAcceptance = new DeliveryLineDTO
                    {
                        DeliveryType = DeliveryLineRouteTypes.Accepting,
                        UrgencyInHours = 24,
                        ReceiverSecretCode = CommonUtility.GetSecretCode(),
                        DeliveryHeaderId = Delivery.Id,
                    };
                    _deliveryService.InsertOrUpdateChild(selectedDeliveryLineAcceptance);
                }

                #region Check Validity
                if (string.IsNullOrEmpty(SenderName))
                {
                    MessageBox.Show("Add Sender Name!");
                    return;
                }

                if (string.IsNullOrEmpty(ReceiverName))
                {
                    MessageBox.Show("Add Receiver Name!");
                    return;
                }

                if (ToAdressDetail.Count == 0)
                {
                    MessageBox.Show("Add Receiver Address!");
                    return;
                }
                #endregion

                if (SelectedSender != null)
                {
                    SelectedDeliveryLine.FromClientId = SelectedSender.Id;
                }
                else
                {
                    var fromAddr = FromAdressDetail.FirstOrDefault();

                    SelectedDeliveryLine.FromClient = new ClientDTO
                    {
                        DisplayName = SenderName,
                        IsReceiver = true,
                        IsActive = true,
                        Type = ClientTypes.Personal,
                        Code = CommonUtility.GetSecretCode(),
                        Address = fromAddr
                    };
                }


                if (SelectedReceiver != null)
                {
                    SelectedDeliveryLine.ToClientId = SelectedReceiver.Id;
                }
                else
                {
                    var toAddr = ToAdressDetail.FirstOrDefault();

                    SelectedDeliveryLine.ToClient = new ClientDTO
                    {
                        DisplayName = ReceiverName,
                        IsReceiver = true,
                        IsActive = true,
                        Type = ClientTypes.Personal,
                        Code = CommonUtility.GetSecretCode(),
                        Address = toAddr
                    };
                }

                _deliveryService.InsertOrUpdateChild(SelectedDeliveryLine);

                CloseWindow(obj);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                  + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand FromAddressViewCommand
        {
            get { return _fromAddressViewCommand ?? (_fromAddressViewCommand = new RelayCommand(FromAddress)); }
        }
        public void FromAddress()
        {
            if (SelectedDeliveryLine != null && SelectedDeliveryLine.FromAddress != null)
            {
                var fromAddr = new AddressDTO
                {
                    Country = "Ethiopia",
                    City = "Addis Abeba"
                };

                fromAddr = MapperUtility<AddressDTO>.GetMap(SelectedDeliveryLine.FromAddress, fromAddr) as AddressDTO;
                if (fromAddr != null)
                {
                    fromAddr.Id = 0;

                    var addr = new AddressEntry(fromAddr);
                    addr.ShowDialog();
                    var dialogueResult = addr.DialogResult;
                    if (dialogueResult != null && (bool)dialogueResult)
                    {
                        SelectedDeliveryLine.FromAddress = fromAddr;
                        FromAdressDetail = new ObservableCollection<AddressDTO> { fromAddr };
                    }
                }
            }
        }

        public ICommand ToAddressViewCommand
        {
            get { return _toAddressViewCommand ?? (_toAddressViewCommand = new RelayCommand(ToAddress)); }
        }
        public void ToAddress()
        {
            if (SelectedDeliveryLine != null)
            {
                var toAddr = new AddressDTO
                {
                    Country = "Ethiopia",
                    City = "Addis Abeba"
                };

                if (SelectedDeliveryLine.ToAddress != null)
                    toAddr = MapperUtility<AddressDTO>.GetMap(SelectedDeliveryLine.ToAddress, toAddr) as AddressDTO;

                if (toAddr != null)
                {
                    var addr = new AddressEntry(toAddr);
                    addr.ShowDialog();
                    var dialogueResult = addr.DialogResult;
                    if (dialogueResult != null && (bool)dialogueResult)
                    {
                        SelectedDeliveryLine.ToAddress = toAddr;
                        ToAdressDetail = new ObservableCollection<AddressDTO> { toAddr };
                    }
                }
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

        #region From/To Person

        private ClientDTO _selectedReceiver;
        private ClientDTO _selectedSender;
        private IEnumerable<ClientDTO> _receiverList;
        private ObservableCollection<ClientDTO> _receivers;

        public ClientDTO SelectedReceiver
        {
            get { return _selectedReceiver; }
            set
            {
                _selectedReceiver = value;
                RaisePropertyChanged<ClientDTO>(() => SelectedReceiver);
                ReceiverName = string.Empty;
                ToAdressDetail = new ObservableCollection<AddressDTO>();
                if (SelectedReceiver != null)
                {
                    ReceiverName = SelectedReceiver.ClientDetail;
                    ToAdressDetail = new ObservableCollection<AddressDTO> { SelectedReceiver.Address };
                    SelectedDeliveryLine.ToAddress = SelectedReceiver.Address;
                }
            }
        }
        public ClientDTO SelectedSender
        {
            get { return _selectedSender; }
            set
            {
                _selectedSender = value;
                RaisePropertyChanged<ClientDTO>(() => SelectedSender);
                SenderName = string.Empty;
                FromAdressDetail = new ObservableCollection<AddressDTO>();
                if (SelectedSender != null)
                {
                    SenderName = SelectedSender.ClientDetail;
                    FromAdressDetail = new ObservableCollection<AddressDTO> { SelectedSender.Address };
                    SelectedDeliveryLine.FromAddress = SelectedSender.Address;
                }
            }
        }
        public IEnumerable<ClientDTO> ReceiverList
        {
            get { return _receiverList; }
            set
            {
                _receiverList = value;
                RaisePropertyChanged<IEnumerable<ClientDTO>>(() => ReceiverList);
            }
        }
        public ObservableCollection<ClientDTO> Receivers
        {
            get { return _receivers; }
            set
            {
                _receivers = value;
                RaisePropertyChanged<ObservableCollection<ClientDTO>>(() => Receivers);
            }
        }
        public void GetReceivers()
        {
            var criteria = new SearchCriteria<ClientDTO>();

            criteria.FiList.Add(b => b.IsActive);//b.IsReceiver &&

            ReceiverList = _clientService.GetAll(criteria).OrderBy(i => i.Id).ToList();

            Receivers = new ObservableCollection<ClientDTO>(ReceiverList);
        }

        #endregion
        
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
