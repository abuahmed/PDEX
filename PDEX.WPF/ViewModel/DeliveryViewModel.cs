using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PDEX.Core;
using PDEX.Core.Common;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;
using PDEX.DAL;
using PDEX.Service;
using PDEX.Service.Interfaces;
using GalaSoft.MvvmLight.Command;
using PDEX.WPF.Reports;
using PDEX.WPF.Reports.DataSets;
using PDEX.WPF.Views;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

namespace PDEX.WPF.ViewModel
{
    public partial class DeliveryViewModel : ViewModelBase
    {
        #region Fields
        private static IDeliveryService _deliveryService;
        private static IClientService _clientService;
        private int _totalNumberOfDelivery;
        private string _acceptanceVisibility;
        private bool _editingEnability;
        private bool _loadData;
        private ObservableCollection<DeliveryHeaderDTO> _transactions;
        private DeliveryHeaderDTO _selectedDelivery;
        private ICommand _addNewDeliveryCommand, _saveDeliveryCommand, _chooseOrderByClientCommand, _saveAcceptanceCommand,
                            _deleteDeliveryCommand, _filterByDateCommand, _refreshWindowCommand, _quickProcessViewCommand;
        #endregion

        #region Constructor

        public DeliveryViewModel()
        {
            FillPeriodCombo();
            SelectedPeriod = FilterPeriods.FirstOrDefault(p => p.Value == 0);//Value == 0(show all) Value=3(show this week)
            FillStatusCombo();
            Load();
            CheckRoles();
            AcceptanceVisibility = "Collapsed";
        }
        public bool LoadData
        {
            get { return _loadData; }
            set
            {
                _loadData = value;
                RaisePropertyChanged<bool>(() => LoadData);
                if (LoadData)
                    Load();
            }
        }
        public void Load()
        {
            CleanUp();
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _deliveryService = new DeliveryService(iDbContext);
            _clientService = new ClientService(iDbContext);
            LoadStaffs();
            GetOrderByClients();
            GetDeliverys();
        }

        public static void CleanUp()
        {
            if (_deliveryService != null)
                _deliveryService.Dispose();
            if (_clientService != null)
                _clientService.Dispose();
        }

        #endregion

        #region Public Properties
        public int TotalNumberOfDelivery
        {
            get { return _totalNumberOfDelivery; }
            set
            {
                _totalNumberOfDelivery = value;
                RaisePropertyChanged<int>(() => TotalNumberOfDelivery);
            }
        }

        public bool EditingEnability
        {
            get { return _editingEnability; }
            set
            {
                _editingEnability = value;
                RaisePropertyChanged<bool>(() => EditingEnability);
            }
        }

        public string AcceptanceVisibility
        {
            get { return _acceptanceVisibility; }
            set
            {
                _acceptanceVisibility = value;
                RaisePropertyChanged<string>(() => AcceptanceVisibility);

                if (AcceptanceVisibility == "Visible")
                {
                    SelectedDeliveryLineAcceptance = _deliveryService.GetChilds(SelectedDelivery.Id, false)
                                    .FirstOrDefault(d => d.DeliveryType == DeliveryLineRouteTypes.Accepting);

                    if (SelectedDeliveryLineAcceptance != null)
                        SelectedStaff = Staffs.FirstOrDefault(s => s.Id == SelectedDeliveryLineAcceptance.ToStaffId);
                    else
                    {
                        SelectedStaff = null;
                        SelectedDeliveryLineAcceptance = new DeliveryLineDTO
                        {
                            DeliveryType = DeliveryLineRouteTypes.Accepting,
                            UrgencyInHours = 24,
                            ReceiverSecretCode = CommonUtility.GetSecretCode(),
                            DeliveryHeaderId = SelectedDelivery.Id,
                        };
                    }
                }
            }
        }

        public IEnumerable<DeliveryHeaderDTO> DeliveryList
        {
            get { return _transactionsList; }
            set
            {
                _transactionsList = value;
                RaisePropertyChanged<IEnumerable<DeliveryHeaderDTO>>(() => DeliveryList);
            }
        }
        public ObservableCollection<DeliveryHeaderDTO> Deliverys
        {
            get { return _transactions; }
            set
            {
                _transactions = value;
                RaisePropertyChanged<ObservableCollection<DeliveryHeaderDTO>>(() => Deliverys);

                GetSummary();

                if (Deliverys != null)
                {
                    var deliveryHeaderDTO = Deliverys.FirstOrDefault();
                    if (deliveryHeaderDTO != null)
                    {
                        //if (deliveryHeaderDTO.OrderByClientId != null)
                        //    AddNewDelivery();
                        //else
                        SelectedDelivery = deliveryHeaderDTO;
                    }
                    else
                    {
                        AddNewDelivery();
                    }
                }
            }
        }
        public DeliveryHeaderDTO SelectedDelivery
        {
            get { return _selectedDelivery; }
            set
            {
                _selectedDelivery = value;
                RaisePropertyChanged<DeliveryHeaderDTO>(() => SelectedDelivery);

                OrderByClientDetail = new ObservableCollection<ClientDTO>();
                DeliveryLines = new ObservableCollection<DeliveryLineDTO>();
                DeliveryLineMessages = new ObservableCollection<MessageDTO>();

                if (SelectedDelivery != null && SelectedDelivery.Id != 0)
                {
                    SelectedClient = SelectedDelivery.OrderByClient;
                    if (SelectedDelivery.OrderByClient != null)
                        OrderByClientDetail.Add(SelectedDelivery.OrderByClient);

                    DeliveryLineMessages = new ObservableCollection<MessageDTO>();

                    GetDeliveryLines();

                    AcceptanceVisibility = SelectedDelivery.AcceptanceType == AcceptanceTypes.InOffice ? "Visible" : "Collapsed";
                    EditingEnability = SelectedDelivery.Status == DeliveryStatusTypes.Ordered ||
                                       SelectedDelivery.Status == DeliveryStatusTypes.AcceptanceScheduled |
                                       SelectedDelivery.Status == DeliveryStatusTypes.OnAcceptance;
                }
            }
        }
        #endregion

        public void GetDeliverys()
        {
            try
            {
                DeliveryList = new List<DeliveryHeaderDTO>();

                var criteria = new SearchCriteria<DeliveryHeaderDTO>
                {
                    CurrentUserId = Singleton.User.UserId,
                    BeginingDate = FilterStartDate,
                    EndingDate = FilterEndDate
                };

                DeliveryList = _deliveryService.GetAll(criteria);
                Deliverys = new ObservableCollection<DeliveryHeaderDTO>(DeliveryList);

            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't Get Deliveries" + Environment.NewLine + exception.Message + Environment.NewLine + exception.InnerException,
                                "Can't Get Deliveries", MessageBoxButton.OK, MessageBoxImage.Error);
                //DeliveryList = new List<DeliveryHeaderDTO>();
                //Deliverys = new ObservableCollection<DeliveryHeaderDTO>(DeliveryList);
            }
        }

        public void GetSummary()
        {
            if (Deliverys != null)
            {
                TotalNumberOfDelivery = Deliverys.Count();
            }
        }

        #region Commands
        public ICommand AddNewDeliveryCommand
        {
            get
            {
                return _addNewDeliveryCommand ?? (_addNewDeliveryCommand = new RelayCommand(AddNewDelivery));
            }
        }
        private void AddNewDelivery()
        {
            try
            {
                SelectedClient = new ClientDTO();
                SelectedDelivery = new DeliveryHeaderDTO
                {
                    OrderDate = DateTime.Now,
                    Status = DeliveryStatusTypes.Ordered,
                    AcceptanceType = AcceptanceTypes.OutsideOffice
                };
                _deliveryService.InsertOrUpdate(SelectedDelivery);

                //SelectedDelivery.Number = "D" + (100000 + SelectedDelivery.Id).ToString(CultureInfo.InvariantCulture).Substring(1);
                //_deliveryService.InsertOrUpdate(SelectedDelivery);

                //ExcuteChooseOrderByClient();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't add new"
                                  + Environment.NewLine + exception.Message, "Can't add new", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand SaveDeliveryCommand
        {
            get
            {
                return _saveDeliveryCommand ?? (_saveDeliveryCommand = new RelayCommand(SaveDelivery));
            }
        }
        private void SaveDelivery()
        {
            try
            {
                var stat = _deliveryService.InsertOrUpdate(SelectedDelivery);
                if (stat == string.Empty)
                {
                    SelectedDelivery = SelectedDelivery;
                }
                else
                    MessageBox.Show("Can't Save Delivery, Please try again!!" + Environment.NewLine + stat,
                                     "Can't save", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                  + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand ChooseOrderByClientCommand
        {
            get
            {
                return _chooseOrderByClientCommand ?? (_chooseOrderByClientCommand = new RelayCommand(ExcuteChooseOrderByClient));
            }
        }
        private void ExcuteChooseOrderByClient()
        {
            try
            {
                new OrderByClientEntry(SelectedDelivery).ShowDialog();

                OrderByClientDetail = new ObservableCollection<ClientDTO>();
                SelectedDelivery.OrderByClient = Clients.FirstOrDefault(oc => oc.Id == SelectedDelivery.OrderByClientId);
                if (SelectedDelivery.OrderByClient != null)
                    OrderByClientDetail.Add(SelectedDelivery.OrderByClient);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't Choose Client"
                                  + Environment.NewLine + exception.Message, "Can't post", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand DeleteDeliveryCommand
        {
            get
            {
                return _deleteDeliveryCommand ?? (_deleteDeliveryCommand = new RelayCommand(ExcuteDeleteDelivery));
            }
        }
        private void ExcuteDeleteDelivery()
        {
            try
            {
                if (MessageBox.Show("Are you Sure You want to Delete this Delivery?", "Delete Delivery",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    SelectedDelivery.Enabled = false;
                    var stat = _deliveryService.Disable(SelectedDelivery);
                    if (stat == string.Empty)
                    {
                        Deliverys.Remove(SelectedDelivery);
                        Deliverys = Deliverys;
                    }
                    else
                        MessageBox.Show("Can't Delete Delivery, Please try again!!" + Environment.NewLine + stat,
                                         "Can't delete", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't Delete Delivery, Please try again!!"
                                    + Environment.NewLine + exception.Message, "Can't delete", MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }

        }

        public ICommand SaveAcceptanceCommand
        {
            get
            {
                return _saveAcceptanceCommand ?? (_saveAcceptanceCommand = new RelayCommand(ExcuteSaveAcceptance));
            }
        }
        public void ExcuteSaveAcceptance()
        {
            string stat;
            try
            {
                #region Validation
                if (SelectedDelivery.CountLines == 0)
                {
                    MessageBox.Show("Add Lines First");
                    return;
                }
                if (SelectedDelivery.CountMessages == 0)
                {
                    MessageBox.Show("Add Messages First First");
                    return;
                }
                if (SelectedStaff.Id == 0)
                {
                    MessageBox.Show("Choose the staff that accepted the messages");
                    return;
                }
                if (SelectedDeliveryLineAcceptance.DeliveredTime == null)
                {
                    MessageBox.Show("Choose the date the message accepted on");
                    return;
                } 
                #endregion

                SelectedDeliveryLineAcceptance.ToStaffId = SelectedStaff.Id;
                stat = _deliveryService.InsertOrUpdateChild(SelectedDeliveryLineAcceptance);
                
                if (string.IsNullOrEmpty(stat))
                {
                    SelectedDelivery.Status = DeliveryStatusTypes.Accepted;
                    stat = _deliveryService.InsertOrUpdate(SelectedDelivery);
                }
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }

            if (!string.IsNullOrEmpty(stat))
                MessageBox.Show(stat);
        }

        public DeliveryLineDTO SelectedDeliveryLineAcceptance
        {
            get { return _selectedDeliveryLineAcceptance; }
            set
            {
                _selectedDeliveryLineAcceptance = value;
                RaisePropertyChanged<DeliveryLineDTO>(() => SelectedDeliveryLineAcceptance);
            }
        }

        public ICommand FilterByDateCommand
        {
            get { return _filterByDateCommand ?? (_filterByDateCommand = new RelayCommand(ExecuteFilterByDateCommand)); }
        }
        private void ExecuteFilterByDateCommand()
        {
            GetDeliverys();
        }

        public ICommand RefreshWindowCommand
        {
            get
            {
                return _refreshWindowCommand ?? (_refreshWindowCommand = new RelayCommand(Load));
            }
        }

        public ICommand QuickProcessViewCommand
        {
            get
            {
                return _quickProcessViewCommand ?? (_quickProcessViewCommand = new RelayCommand(ExcuteQuickProcessViewCommand));
            }
        }
        private void ExcuteQuickProcessViewCommand()
        {
            try
            {
                new QuickProcess(ProcessTypes.Delivery, SelectedDelivery.Id).ShowDialog();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't Process" + Environment.NewLine + exception.Message, "Can't Process", 
                                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Acceptance Confirmation

        private ObservableCollection<StaffDTO> _staffs;
        private StaffDTO _selectedStaff;

        public void LoadStaffs()
        {
            var criteria = new SearchCriteria<StaffDTO>();
            //criteria.FiList.Add(c => c.NameType == NameTypes.UnitOfMeasure);
            IEnumerable<StaffDTO> staffsList = new StaffService(true)
                .GetAll(criteria)
                .OrderBy(i => i.Id)
                .ToList();

            Staffs = new ObservableCollection<StaffDTO>(staffsList);
        }
        public StaffDTO SelectedStaff
        {
            get { return _selectedStaff; }
            set
            {
                _selectedStaff = value;
                RaisePropertyChanged<StaffDTO>(() => SelectedStaff);
            }
        }
        public ObservableCollection<StaffDTO> Staffs
        {
            get { return _staffs; }
            set
            {
                _staffs = value;
                RaisePropertyChanged<ObservableCollection<StaffDTO>>(() => Staffs);
            }
        }
        #endregion

        #region Filter List

        #region Fields
        private IEnumerable<DeliveryHeaderDTO> _transactionsList;
        private ObservableCollection<ListDataItem> _filterPeriods, _filterStatus;
        private ListDataItem _selectedPeriod, _selectedStatus;
        private DeliveryHeaderDTO _selectedDeliveryForFilter;
        private string _filterPeriod;
        private DateTime _filterStartDate, _filterEndDate;
        #endregion

        #region By Period
        public string FilterPeriod
        {
            get { return _filterPeriod; }
            set
            {
                _filterPeriod = value;
                RaisePropertyChanged<string>(() => FilterPeriod);
            }
        }
        public DateTime FilterStartDate
        {
            get { return _filterStartDate; }
            set
            {
                _filterStartDate = value;
                RaisePropertyChanged<DateTime>(() => FilterStartDate);
            }
        }
        public DateTime FilterEndDate
        {
            get { return _filterEndDate; }
            set
            {
                _filterEndDate = value;
                RaisePropertyChanged<DateTime>(() => FilterEndDate);
            }
        }

        private void FillPeriodCombo()
        {
            IList<ListDataItem> filterPeriods2 = new List<ListDataItem>();
            filterPeriods2.Add(new ListDataItem { Display = "All", Value = 0 });
            filterPeriods2.Add(new ListDataItem { Display = "Today", Value = 1 });
            filterPeriods2.Add(new ListDataItem { Display = "Yesterday", Value = 2 });
            filterPeriods2.Add(new ListDataItem { Display = "This Week", Value = 3 });
            filterPeriods2.Add(new ListDataItem { Display = "Last Week", Value = 4 });
            FilterPeriods = new ObservableCollection<ListDataItem>(filterPeriods2);
        }
        public ObservableCollection<ListDataItem> FilterPeriods
        {
            get { return _filterPeriods; }
            set
            {
                _filterPeriods = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => FilterPeriods);
            }
        }
        public ListDataItem SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                _selectedPeriod = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedPeriod);
                if (SelectedPeriod != null)
                {
                    switch (SelectedPeriod.Value)
                    {
                        case 0:
                            FilterStartDate = DateTime.Now.AddYears(-1);
                            FilterEndDate = DateTime.Now;
                            break;
                        case 1:
                            FilterStartDate = DateTime.Now;
                            FilterEndDate = DateTime.Now;
                            break;
                        case 2:
                            FilterStartDate = DateTime.Now.AddDays(-1);
                            FilterEndDate = DateTime.Now.AddDays(-1);
                            break;
                        case 3:
                            FilterStartDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                            FilterEndDate = DateTime.Now.AddDays(7 - (int)DateTime.Now.DayOfWeek - 1);
                            break;
                        case 4:
                            FilterStartDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 7);
                            FilterEndDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 1);
                            break;
                    }
                }
            }
        }

        #endregion

        #region By Status
        private void FillStatusCombo()
        {
            var filterStatus2 = new List<ListDataItem>
            {
                new ListDataItem {Display = "All", Value = 0},
                //new ListDataItem {Display = EnumUtil.GetEnumDesc(PaymentStatus.NoPayment), Value = 1},
                //new ListDataItem {Display = EnumUtil.GetEnumDesc(PaymentStatus.NotCleared), Value = 2},
                //new ListDataItem {Display = EnumUtil.GetEnumDesc(PaymentStatus.Cleared), Value = 3}
            };
            FilterStatus = new ObservableCollection<ListDataItem>(filterStatus2);
        }
        public ObservableCollection<ListDataItem> FilterStatus
        {
            get { return _filterStatus; }
            set
            {
                _filterStatus = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => FilterStatus);
            }
        }
        public ListDataItem SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                _selectedStatus = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedStatus);
                GetDeliverys();
            }
        }
        #endregion

        #region By Delivery
        public DeliveryHeaderDTO SelectedDeliveryForFilter
        {
            get { return _selectedDeliveryForFilter; }
            set
            {
                _selectedDeliveryForFilter = value;
                RaisePropertyChanged<DeliveryHeaderDTO>(() => SelectedDeliveryForFilter);

                if (SelectedDeliveryForFilter != null)
                {
                    var criteria = new SearchCriteria<DeliveryHeaderDTO>
                    {
                        CurrentUserId = Singleton.User.UserId
                    };
                    DeliveryList = _deliveryService.GetAll(criteria);

                    int id;
                    var idtr = Int32.TryParse(SelectedDeliveryForFilter.Number.Substring(1), out id);
                    Deliverys = idtr
                        ? new ObservableCollection<DeliveryHeaderDTO>(DeliveryList.Where(s => s.Id == id))
                        : new ObservableCollection<DeliveryHeaderDTO>(DeliveryList);
                }
                else
                {
                    GetDeliverys();
                }
            }
        }
        #endregion

        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave()
        {
            return Errors == 0;
        }

        public static int LineErrors { get; set; }
        public bool CanSaveLine(object obj)
        {
            return LineErrors == 0;
        }
        #endregion

        #region Privilege Visibility
        private UserRolesModel _userRoles;
        private string _addDelivery, _editDelivery, _deleteDelivery, _acceptDelivery,
                        _addDeliveryLine, _editDeliveryLine, _deleteDeliveryLine,
                        _addLineMessage, _editLineMessage, _deleteLineMessage,
                        _printDeliveryVisibility;

        public UserRolesModel UserRoles
        {
            get { return _userRoles; }
            set
            {
                _userRoles = value;
                RaisePropertyChanged<UserRolesModel>(() => UserRoles);
            }
        }

        public string AddDelivery
        {
            get { return _addDelivery; }
            set
            {
                _addDelivery = value;
                RaisePropertyChanged<string>(() => AddDelivery);
            }
        }
        public string EditDelivery
        {
            get { return _editDelivery; }
            set
            {
                _editDelivery = value;
                RaisePropertyChanged<string>(() => EditDelivery);
            }
        }
        public string DeleteDelivery
        {
            get { return _deleteDelivery; }
            set
            {
                _deleteDelivery = value;
                RaisePropertyChanged<string>(() => DeleteDelivery);
            }
        }
        public string AcceptDelivery
        {
            get { return _acceptDelivery; }
            set
            {
                _acceptDelivery = value;
                RaisePropertyChanged<string>(() => AcceptDelivery);
            }
        }

        public string AddDeliveryLine
        {
            get { return _addDeliveryLine; }
            set
            {
                _addDeliveryLine = value;
                RaisePropertyChanged<string>(() => AddDeliveryLine);
            }
        }
        public string EditDeliveryLine
        {
            get { return _editDeliveryLine; }
            set
            {
                _editDeliveryLine = value;
                RaisePropertyChanged<string>(() => EditDeliveryLine);
            }
        }
        public string DeleteDeliveryLine
        {
            get { return _deleteDeliveryLine; }
            set
            {
                _deleteDeliveryLine = value;
                RaisePropertyChanged<string>(() => DeleteDeliveryLine);
            }
        }

        public string AddLineMessage
        {
            get { return _addLineMessage; }
            set
            {
                _addLineMessage = value;
                RaisePropertyChanged<string>(() => AddLineMessage);
            }
        }
        public string EditLineMessage
        {
            get { return _editLineMessage; }
            set
            {
                _editLineMessage = value;
                RaisePropertyChanged<string>(() => EditLineMessage);
            }
        }
        public string DeleteLineMessage
        {
            get { return _deleteLineMessage; }
            set
            {
                _deleteLineMessage = value;
                RaisePropertyChanged<string>(() => DeleteLineMessage);
            }
        }

        public string PrintDeliveryVisibility
        {
            get { return _printDeliveryVisibility; }
            set
            {
                _printDeliveryVisibility = value;
                RaisePropertyChanged<string>(() => PrintDeliveryVisibility);
            }
        }

        private void CheckRoles()
        {
            UserRoles = Singleton.UserRoles;
            if (UserRoles != null)
            {
                AddDelivery = UserRoles.AddDelivery;
                EditDelivery = UserRoles.EditDelivery;
                DeleteDelivery = UserRoles.DeleteDelivery;
                AcceptDelivery = UserRoles.AcceptDelivery;

                AddDeliveryLine = UserRoles.AddLine;
                EditDeliveryLine = UserRoles.EditLine;
                DeleteDeliveryLine = UserRoles.DeleteLine;

                AddLineMessage = UserRoles.AddMessage;
                EditLineMessage = UserRoles.EditMessage;
                DeleteLineMessage = UserRoles.DeleteMessage;
            }
        }
        #endregion

        #region Print Attachment
        public byte[] ImageToByteArray(Image imageIn, ImageFormat format)
        {
            var ms = new MemoryStream();
            imageIn.Save(ms, format);
            return ms.ToArray();
        }
        private ICommand _printDeliveryCommand;
        public ICommand PrintDeliveryCommand
        {
            get
            {
                return _printDeliveryCommand ?? (_printDeliveryCommand = new RelayCommand<Object>(PrintDelivery));
            }
        }
        public void PrintDelivery(object obj)
        {
            var myReport = new DeliveryForm();
            myReport.SetDataSource(GetListDataSet());

            var report = new ReportViewerCommon(myReport);
            report.Show();
        }

        public DeliveryDataSet GetListDataSet()
        {
            var myDataSet = new DeliveryDataSet();
            var brCode = new BarcodeProcess();

            try
            {
                var company = new CompanyService(true).GetCompany();

                var lines = new DeliveryService().GetChilds(SelectedDelivery.Id, true)
                    .Where(d => d.DeliveryType == DeliveryLineRouteTypes.Delivering).ToList();

                foreach (var deliveryLineDTO in lines)
                {
                    var orderNum = "" + SelectedDelivery.Number.Substring(1) + "_" + deliveryLineDTO.Number.Substring(1);
                    var linkno = deliveryLineDTO.Id;
                    var deliverybarcode = ImageToByteArray(brCode.GetBarcode(orderNum, 320, 40, false), ImageFormat.Bmp);
                    myDataSet.ClientDetail.Rows.Add(
                        company.Header,
                        company.Footer,
                        company.Address.AddressDetail,
                        company.Address.SubCity,
                        company.Address.Kebele,
                        company.Address.HouseNumber,
                        company.Address.Telephone,
                        company.Address.Mobile,
                        company.Address.Fax,
                        company.Address.PrimaryEmail,
                        company.Address.AlternateEmail,
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        "",
                        linkno.ToString(CultureInfo.InvariantCulture));

                    myDataSet.DeliveryLine.Rows.Add(
                        "1",
                        SelectedDelivery.Number + "-" + deliveryLineDTO.Number,
                        SelectedDelivery.OrderDateString,
                        "",
                        SelectedDelivery.OrderByClient.DisplayName,
                        SelectedDelivery.OrderByClient.Address.AddressDetail,
                        SelectedDelivery.OrderByClient.Address.Mobile,
                        SelectedDelivery.OrderByClient.Address.AlternateMobile,
                        SelectedDelivery.OrderByClient.Address.Telephone,
                        deliveryLineDTO.FromClient.DisplayName,
                        deliveryLineDTO.FromAddress.AddressDetail,
                        deliveryLineDTO.FromAddress.Mobile,
                        deliveryLineDTO.FromAddress.AlternateMobile,
                        deliveryLineDTO.FromAddress.Telephone,
                        deliveryLineDTO.ToClient.DisplayName,
                        deliveryLineDTO.ToAddress.AddressDetail,
                        deliveryLineDTO.ToAddress.Mobile,
                        deliveryLineDTO.ToAddress.AlternateMobile,
                        deliveryLineDTO.ToAddress.Telephone,
                        deliverybarcode,
                        "",
                        "",
                        "",
                        "",
                        "",
                        linkno.ToString(CultureInfo.InvariantCulture)
                        );

                    var serNo = 1;
                    foreach (var message in deliveryLineDTO.Messages)
                    {
                        myDataSet.DeliveryMessage.Rows.Add(
                            serNo,
                            message.Number,
                            message.Category.DisplayName,
                            message.UnitOfMeasure.DisplayName,
                            message.Description,
                            message.Unit,
                            "",
                            "",
                            "",
                            "",
                            "",
                            null,
                            linkno.ToString(CultureInfo.InvariantCulture));
                        serNo++;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't get data for the report"
                                  + Environment.NewLine + exception.Message, "Can't get data", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }

            return myDataSet;
        }
        #endregion
    }
}
