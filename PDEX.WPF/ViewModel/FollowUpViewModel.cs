using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PDEX.Core;
using PDEX.Core.Common;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;
using PDEX.Service;
using PDEX.Service.Interfaces;
using PDEX.WPF.Views;

namespace PDEX.WPF.ViewModel
{
    public class FollowUpViewModel : ViewModelBase
    {
        #region Fields
        private static IDeliveryService _deliveryService;
        private int _totalNumberOfDelivery;
        private bool _loadData;
        private ObservableCollection<DeliveryHeaderDTO> _onRequestdeliverys, _onAcceptancedeliverys, _deliverys;
        private ObservableCollection<DeliveryLineDTO> _deliveryLines;
        private DeliveryHeaderDTO _selectedDelivery;
        private DeliveryLineDTO _selectedDeliveryLine;
        private ICommand _addNewDeliveryCommand, _saveDeliveryCommand, _chooseOrderByClientCommand,
                         _filterByDateCommand, _refreshWindowCommand;
        #endregion

        #region Constructor

        public FollowUpViewModel()
        {
            FillPeriodCombo();
            SelectedPeriod = FilterPeriods.FirstOrDefault(p => p.Value == 0);//Value == 0(show all) Value=3(show this week)
            FillStatusCombo();
            Load();
            CheckRoles();
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
            _deliveryService = new DeliveryService();
            LoadStaffs();
            GetDeliverys();
        }

        public static void CleanUp()
        {
            if (_deliveryService != null)
                _deliveryService.Dispose();
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


        public IEnumerable<DeliveryHeaderDTO> DeliveryList
        {
            get { return _deliverysList; }
            set
            {
                _deliverysList = value;
                RaisePropertyChanged<IEnumerable<DeliveryHeaderDTO>>(() => DeliveryList);
            }
        }
        public ObservableCollection<DeliveryHeaderDTO> OnRequestDeliverys
        {
            get { return _onRequestdeliverys; }
            set
            {
                _onRequestdeliverys = value;
                RaisePropertyChanged<ObservableCollection<DeliveryHeaderDTO>>(() => OnRequestDeliverys);

                GetSummary();
            }
        }
        public ObservableCollection<DeliveryHeaderDTO> OnAcceptanceDeliverys
        {
            get { return _onAcceptancedeliverys; }
            set
            {
                _onAcceptancedeliverys = value;
                RaisePropertyChanged<ObservableCollection<DeliveryHeaderDTO>>(() => OnAcceptanceDeliverys);

                GetSummary();
            }
        }
        public ObservableCollection<DeliveryHeaderDTO> Deliverys
        {
            get { return _deliverys; }
            set
            {
                _deliverys = value;
                RaisePropertyChanged<ObservableCollection<DeliveryHeaderDTO>>(() => Deliverys);

                GetSummary();
            }
        }
        public DeliveryHeaderDTO SelectedDelivery
        {
            get { return _selectedDelivery; }
            set
            {
                _selectedDelivery = value;
                RaisePropertyChanged<DeliveryHeaderDTO>(() => SelectedDelivery);

            }
        }

        public ObservableCollection<DeliveryLineDTO> DeliveryLines
        {
            get { return _deliveryLines; }
            set
            {
                _deliveryLines = value;
                RaisePropertyChanged<ObservableCollection<DeliveryLineDTO>>(() => DeliveryLines);

            }
        }
        public DeliveryLineDTO SelectedDeliveryLine
        {
            get { return _selectedDeliveryLine; }
            set
            {
                _selectedDeliveryLine = value;
                RaisePropertyChanged<DeliveryLineDTO>(() => SelectedDeliveryLine);

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
                    //BeginingDate = FilterStartDate,
                    //EndingDate = FilterEndDate
                };
                criteria.FiList.Add(d => d.OrderByClient != null);
                DeliveryList = _deliveryService.GetAll(criteria).ToList();

                DeliveryList = DeliveryList.Where(d => d.CountLines > 0 &&
                                                       d.CountMessages > 0).ToList();

                OnRequestDeliverys = new ObservableCollection<DeliveryHeaderDTO>(
                    DeliveryList.Where(d => d.Status == DeliveryStatusTypes.Ordered).ToList());

                OnAcceptanceDeliverys = new ObservableCollection<DeliveryHeaderDTO>(
                    DeliveryList.Where(d => d.Status == DeliveryStatusTypes.OnAcceptance ||
                        d.Status == DeliveryStatusTypes.AcceptanceScheduled).ToList());

                Deliverys = new ObservableCollection<DeliveryHeaderDTO>(
                    DeliveryList.Where(d => d.Status == DeliveryStatusTypes.Accepted ||
                        d.Status == DeliveryStatusTypes.DeliveryScheduled || d.Status == DeliveryStatusTypes.OnDelivery).ToList());

                IList<DeliveryLineDTO> delLines = Deliverys.SelectMany(deliveryHeaderDTO => deliveryHeaderDTO.DeliveryLines
                                                           .Where(l => l.DeliveryType == DeliveryLineRouteTypes.Delivering))
                                                           .OrderByDescending(l => l.DeliveryHeaderId).ToList();
                
                DeliveryLines = new ObservableCollection<DeliveryLineDTO>(delLines);

            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't Get Deliveries"
                                + Environment.NewLine + exception.Message, "Can't Get Deliveries", MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
                if(SelectedDelivery==null)
                    return;
                new AcceptanceEntry(SelectedDelivery).ShowDialog();
                Load();
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

        public void SaveDelivery()
        {
            try
            {
                if (SelectedDeliveryLine == null)
                    return;
                new DeliveryEntry(SelectedDeliveryLine).ShowDialog();
                Load();
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

        public void ExcuteChooseOrderByClient()
        {
            try
            {
                
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't Choose OrderByClient"
                                + Environment.NewLine + exception.Message, "Can't post", MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
        #endregion

        #region Staffs

        private ObservableCollection<StaffDTO> _staffs;
        private StaffDTO _selectedStaff;

        public void LoadStaffs()
        {
            var criteria = new SearchCriteria<StaffDTO>();
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
        private IEnumerable<DeliveryHeaderDTO> _deliverysList;
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

                Deliverys = SelectedDeliveryForFilter != null
                    ? new ObservableCollection<DeliveryHeaderDTO>(DeliveryList.Where(s => s.Number == SelectedDeliveryForFilter.Number))
                    : new ObservableCollection<DeliveryHeaderDTO>(DeliveryList);
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

        #region Get Attachment
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
            //new AttachmentEntry(SelectedDelivery).ShowDialog();
        }
        #endregion
    }
}