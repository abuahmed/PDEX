using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PDEX.Core;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;
using PDEX.Service;
using GalaSoft.MvvmLight.Command;
using PDEX.WPF.Views;

namespace PDEX.WPF.ViewModel
{
    public partial class DeliveryViewModel
    {
        #region Lines

        #region Fields
        private bool _isEdit;
        private ICommand _addDeliveryLineCommand, _editDeliveryLineCommand, _deleteDeliveryLineCommand;
        private ICommand _addMessageCommand, _editMessageCommand, _deleteMessageCommand;
        private ObservableCollection<DeliveryLineDTO> _deliveryLines;
        private ObservableCollection<MessageDTO> _deliveryLineMessages;
        private DeliveryLineDTO _selectedDeliveryLine, _selectedDeliveryLineAcceptance;
        private MessageDTO _selectedDeliveryLineMessage;
        #endregion

        #region Public Properties

        public DeliveryLineDTO SelectedDeliveryLine
        {
            get { return _selectedDeliveryLine; }
            set
            {
                _selectedDeliveryLine = value;
                RaisePropertyChanged<DeliveryLineDTO>(() => SelectedDeliveryLine);
                if (SelectedDeliveryLine != null)
                {
                    GetMessages();
                }
            }
        }
        public ObservableCollection<DeliveryLineDTO> DeliveryLines
        {
            get { return _deliveryLines; }
            set
            {
                _deliveryLines = value;
                RaisePropertyChanged<ObservableCollection<DeliveryLineDTO>>(() => DeliveryLines);

                if (DeliveryLines != null)
                {
                    var sNo = 1;
                    foreach (var deliveryLineDTO in DeliveryLines)
                    {
                        deliveryLineDTO.SerialNumber = sNo;
                        sNo++;
                    }
                    SelectedDeliveryLine = DeliveryLines.FirstOrDefault();
                }
                else
                {
                    //DeliveryLines=new ObservableCollection<DeliveryLineDTO>();
                    DeliveryLineMessages = new ObservableCollection<MessageDTO>();
                }
            }
        }

        #endregion

        public void GetDeliveryLines()
        {
            DeliveryLines = new ObservableCollection<DeliveryLineDTO>();
            DeliveryLineMessages = new ObservableCollection<MessageDTO>();

            if (SelectedDelivery != null && SelectedDelivery.Id != 0)
            {
                var lines = new DeliveryService().GetChilds(SelectedDelivery.Id, true)
                    .Where(d => d.DeliveryType == DeliveryLineRouteTypes.Delivering).ToList();
              
                DeliveryLines = new ObservableCollection<DeliveryLineDTO>(lines);
            }
        }

        #region Commands
        public ICommand AddNewDeliveryLineCommand
        {
            get
            {
                return _addDeliveryLineCommand ?? (_addDeliveryLineCommand = new RelayCommand(AddLine));
            }
        }
        private void AddLine()
        {
            if (SelectedDelivery == null)
            {
                MessageBox.Show("Add Header First");
                return;
            }

            if (SelectedDelivery.OrderByClient == null)
            {
                ExcuteChooseOrderByClient();
                return;
            }

            try
            {
                new DeliveryLineEntry(SelectedDelivery).ShowDialog();
                GetDeliveryLines();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Problem adding transaction item, please try again..." + Environment.NewLine +
                    exception.Message + Environment.NewLine +
                    exception.InnerException);
            }
        }

        public ICommand EditDeliveryLineCommand
        {
            get
            {
                return _editDeliveryLineCommand ?? (_editDeliveryLineCommand = new RelayCommand(EditLine, CanSave));
            }
        }
        private void EditLine()
        {
            try
            {
                _isEdit = true;

                if (SelectedDeliveryLine != null)
                {
                    new DeliveryLineEntry(SelectedDeliveryLine).ShowDialog();
                    GetDeliveryLines();
                }
            }
            catch
            {
                MessageBox.Show("Can't Edit Item please try again...");
            }
        }

        public ICommand DeleteDeliveryLineCommand
        {
            get
            {
                return _deleteDeliveryLineCommand ?? (_deleteDeliveryLineCommand = new RelayCommand<Object>(DeleteLine));
            }
        }
        private void DeleteLine(object obj)
        {
            if (SelectedDeliveryLine == null)
            {
                MessageBox.Show("Select Line To Delete");
                return;
            }

            if (MessageBox.Show("Are you Sure You want to Delete this Line, All its Messages will also be deleted?", "Delete Line",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    //SelectedDeliveryLine.Enabled = false;
                    var stat = new DeliveryService(true).DisableChild(SelectedDeliveryLine);
                    if (!string.IsNullOrEmpty(stat))
                        MessageBox.Show(stat);
                    GetDeliveryLines();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Can't Delete, may be the data is already in use..."
                                    + Environment.NewLine + exception.Message + Environment.NewLine +
                                    exception.InnerException, "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #endregion

        #region Messages

        #region Public Properties
        public MessageDTO SelectedDeliveryLineMessage
        {
            get { return _selectedDeliveryLineMessage; }
            set
            {
                _selectedDeliveryLineMessage = value;
                RaisePropertyChanged<MessageDTO>(() => SelectedDeliveryLineMessage);
            }
        }

        public ObservableCollection<MessageDTO> DeliveryLineMessages
        {
            get { return _deliveryLineMessages; }
            set
            {
                _deliveryLineMessages = value;
                RaisePropertyChanged<ObservableCollection<MessageDTO>>(() => DeliveryLineMessages);
                if (DeliveryLineMessages != null)
                {
                    var sNo = 1;
                    foreach (var deliveryLineMessageDTO in DeliveryLineMessages)
                    {
                        deliveryLineMessageDTO.SerialNumber = sNo;
                        sNo++;
                    }
                    var lineMessageCounts = DeliveryLineMessages.Count;
                }
            }
        }
        #endregion

        public void GetMessages()
        {
            if (SelectedDeliveryLine != null)
            {
                var messages = new DeliveryService().GetMessageChilds(SelectedDeliveryLine.Id, true);
                if (messages != null)
                    DeliveryLineMessages = new ObservableCollection<MessageDTO>(messages);
            }
        }

        #region Commands
        public ICommand AddNewMessageCommand
        {
            get
            {
                return _addMessageCommand ?? (_addMessageCommand = new RelayCommand(AddNewMessage));
            }
        }
        private void AddNewMessage()
        {
            if (SelectedDeliveryLine != null)
            {
                new MessageEntry(new MessageDTO()
                {
                    Unit = 1,
                    //Number = CommonUtility.GetSecretCode(),
                    DeliveryLineId = SelectedDeliveryLine.Id
                }).ShowDialog();

                GetMessages();
            }
        }

        public ICommand EditMessageCommand
        {
            get
            {
                return _editMessageCommand ?? (_editMessageCommand = new RelayCommand(EditMessage));
            }
        }
        private void EditMessage()
        {
            if (SelectedDeliveryLineMessage != null)
            {
                new MessageEntry(SelectedDeliveryLineMessage).ShowDialog();
                GetMessages();
            }

        }

        public ICommand DeleteMessageCommand
        {
            get
            {
                return _deleteMessageCommand ?? (_deleteMessageCommand = new RelayCommand(DeleteMessage));
            }
        }
        private void DeleteMessage()
        {
            if (SelectedDeliveryLineMessage == null)
            {
                MessageBox.Show("Select Message To Delete");
                return;
            }

            if (MessageBox.Show("Are you Sure You want to Delete this Message?", "Delete Message",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    //SelectedDeliveryLineMessage.Enabled = false;
                    var stat = new DeliveryService(true).DisableMessageChild(SelectedDeliveryLineMessage);
                    if (!string.IsNullOrEmpty(stat))
                        MessageBox.Show(stat);
                    GetMessages();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Can't Delete, may be the data is already in use..."
                                    + Environment.NewLine + exception.Message + Environment.NewLine +
                                    exception.InnerException, "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #endregion

        #region Payments
        //private ObservableCollection<PaymentDTO> _payments;
        private string _paymentListVisibility;

        //public ObservableCollection<PaymentDTO> Payments
        //{
        //    get { return _payments; }
        //    set
        //    {
        //        _payments = value;
        //        RaisePropertyChanged<ObservableCollection<PaymentDTO>>(() => Payments);
        //        PaymentListVisibility = Payments.Count > 0 ? "Visible" : "Collapsed";
        //    }
        //}
        public string PaymentListVisibility
        {
            get { return _paymentListVisibility; }
            set
            {
                _paymentListVisibility = value;
                RaisePropertyChanged<string>(() => PaymentListVisibility);
            }
        }

        private void GetPayments()
        {

        }
        #endregion

        #region Clients

        #region Fields
        private ObservableCollection<ClientDTO> _clients, _clientsForFilter, _senderDetail;
        private ClientDTO _selectedClient, _selectedClientForFilter;
        private ClientTypes _clientType;
        #endregion

        #region Public Properties
        public ObservableCollection<ClientDTO> Clients
        {
            get { return _clients; }
            set
            {
                _clients = value;
                RaisePropertyChanged<ObservableCollection<ClientDTO>>(() => Clients);
            }
        }
        public ClientDTO SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                RaisePropertyChanged<ClientDTO>(() => SelectedClient);
            }
        }

        public ObservableCollection<ClientDTO> ClientsForFilter
        {
            get { return _clientsForFilter; }
            set
            {
                _clientsForFilter = value;
                RaisePropertyChanged<ObservableCollection<ClientDTO>>(() => ClientsForFilter);
            }
        }
        public ClientDTO SelectedClientForFilter
        {
            get { return _selectedClientForFilter; }
            set
            {
                _selectedClientForFilter = value;
                RaisePropertyChanged<ClientDTO>(() => SelectedClientForFilter);
                if (SelectedClientForFilter != null)
                    GetDeliverys();
            }
        }

        public ClientTypes ClientType
        {
            get { return _clientType; }
            set
            {
                _clientType = value;
                RaisePropertyChanged<ClientTypes>(() => ClientType);
            }
        }

        public ObservableCollection<ClientDTO> OrderByClientDetail
        {
            get { return _senderDetail; }
            set
            {
                _senderDetail = value;
                RaisePropertyChanged<ObservableCollection<ClientDTO>>(() => OrderByClientDetail);
            }
        }
        #endregion

        public void GetOrderByClients()
        {
            try
            {
                var criteria = new SearchCriteria<ClientDTO>();
                var bpList = _clientService
                    .GetAll(criteria)
                    .OrderBy(i => i.Id)
                    .ToList();

                Clients = new ObservableCollection<ClientDTO>(bpList);

                if (bpList.Count > 1)
                    bpList.Insert(0, new ClientDTO
                    {
                        DisplayName = "All",
                        Id = -1
                    });

                ClientsForFilter = new ObservableCollection<ClientDTO>(bpList);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't Load " + ClientType
                                  + Environment.NewLine + exception.Message, "Can't Get " + ClientType, MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
