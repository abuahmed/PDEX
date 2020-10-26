using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

namespace PDEX.WPF.ViewModel
{
    public class QuickProcessViewModel : ViewModelBase
    {
        #region Fields
        private static IDeliveryService _deliveryService;
        private static ITaskProcessService _taskProcessService;
        private ProcessTypes _selectedProcessType;
        private int _processId;

        private WarehouseDTO _selectedWarehouse;
        private DeliveryHeaderDTO _selectedDelivery;
        private DeliveryLineDTO _selectedDeliveryLine;
        private DeliveryRouteDTO _selectedDeliveryRoute;
        private TaskProcessDTO _selectedTaskProcess;

        private string _headerText, _deliverDirectlyVisibiity;
        private bool _deliverDirectly;
        private bool _isAcceptance;
        private ICommand _saveDeliveryLineViewCommand, _closeDeliveryLineViewCommand;//, _resetDeliveryLineViewCommand;
        #endregion

        #region Constructor
        public QuickProcessViewModel()
        {
            CleanUp();
            _deliveryService = new DeliveryService();
            _taskProcessService = new TaskProcessService();

            LoadStaffs();
            LoadVehicles();
            LoadDeliveryMethods();
            CheckRoles();

            SelectedWarehouse = new WarehouseService(true)
                                .GetAll()
                                .FirstOrDefault();

            Messenger.Default.Register<int>(this, (message) =>
            {
                ProcessId = message;
            });

            Messenger.Default.Register<ProcessTypes>(this, (message) =>
            {
                SelectedProcessType = message;
            });
        }

        public static void CleanUp()
        {
            if (_deliveryService != null)
                _deliveryService.Dispose();
        }
        #endregion

        #region Public Properties

        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                _headerText = value;
                RaisePropertyChanged<string>(() => HeaderText);
            }
        }
        public bool DeliverDirectly
        {
            get { return _deliverDirectly; }
            set
            {
                _deliverDirectly = value;
                RaisePropertyChanged<bool>(() => DeliverDirectly);
            }
        }
        public string DeliverDirectlyVisibiity
        {
            get { return _deliverDirectlyVisibiity; }
            set
            {
                _deliverDirectlyVisibiity = value;
                RaisePropertyChanged<string>(() => DeliverDirectlyVisibiity);
            }
        }
        public int ProcessId
        {
            get { return _processId; }
            set
            {
                _processId = value;
                RaisePropertyChanged<int>(() => ProcessId);
            }
        }

        public WarehouseDTO SelectedWarehouse
        {
            get { return _selectedWarehouse; }
            set
            {
                _selectedWarehouse = value;
                RaisePropertyChanged<WarehouseDTO>(() => SelectedWarehouse);
            }
        }
        public ProcessTypes SelectedProcessType
        {
            get
            {
                return _selectedProcessType;
            }
            set
            {
                _selectedProcessType = value;
                RaisePropertyChanged<ProcessTypes>(() => SelectedProcessType);
                if (ProcessId != 0)
                {
                    DeliverDirectlyVisibiity = "Collapsed";
                    switch (SelectedProcessType)
                    {
                        case ProcessTypes.TaskProcess:
                            SelectedTaskProcess = _taskProcessService.Find(ProcessId.ToString(CultureInfo.InvariantCulture));
                            HeaderText = "Process Task";
                            break;
                        case ProcessTypes.Delivery:
                            SelectedDelivery = _deliveryService.GetAll().FirstOrDefault(d => d.Id == ProcessId);
                            if (SelectedDelivery != null)
                            {
                                if (SelectedDelivery.Status == DeliveryStatusTypes.Ordered ||
                                    SelectedDelivery.Status == DeliveryStatusTypes.AcceptanceScheduled ||
                                    SelectedDelivery.Status == DeliveryStatusTypes.OnAcceptance)
                                {
                                    DeliverDirectlyVisibiity = "Visible";
                                    HeaderText = "Acceptance (መልእክት መቀበል)";
                                    _isAcceptance = true;
                                }

                                else if (SelectedDelivery.Status == DeliveryStatusTypes.Accepted ||
                                         SelectedDelivery.Status == DeliveryStatusTypes.DeliveryScheduled ||
                                         SelectedDelivery.Status == DeliveryStatusTypes.OnDelivery)
                                {
                                    HeaderText = "Delivery (መልእክት ማድረስ)";
                                }
                            }
                            break;
                        case ProcessTypes.DeliveryLine:
                            var criteria = new SearchCriteria<DeliveryLineDTO>
                            {
                                CurrentUserId = Singleton.User.UserId
                            };
                            criteria.FiList.Add(dl => dl.Id == ProcessId);
                            int totcount;
                            SelectedDeliveryLine = _deliveryService.GetAllChilds(criteria, out totcount).FirstOrDefault();
                            HeaderText = "Delivery (መልእክት ማድረስ)";
                            break;
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
                if (SelectedDelivery != null)
                {
                    if (SelectedProcessType == ProcessTypes.Delivery)
                    {
                        if (_isAcceptance)
                        {
                            SelectedDeliveryLine =
                                SelectedDelivery.DeliveryLines.FirstOrDefault(
                                    d => d.DeliveryType == DeliveryLineRouteTypes.Accepting);
                            if (SelectedDeliveryLine != null)
                                SelectedDeliveryRoute =
                                    _deliveryService.GetDeliveryRouteChilds(SelectedDeliveryLine.Id, false)
                                        .FirstOrDefault();
                        }
                        else if (!_isAcceptance)
                        {
                            SelectedDeliveryLine =
                                SelectedDelivery.DeliveryLines.FirstOrDefault(
                                    d => d.DeliveryType == DeliveryLineRouteTypes.Delivering);
                            if (SelectedDeliveryLine != null)
                                SelectedDeliveryRoute =
                                    _deliveryService.GetDeliveryRouteChilds(SelectedDeliveryLine.Id, false)
                                        .FirstOrDefault();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Delivery...");
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
            }
        }
        public DeliveryRouteDTO SelectedDeliveryRoute
        {
            get { return _selectedDeliveryRoute; }
            set
            {
                _selectedDeliveryRoute = value;
                RaisePropertyChanged<DeliveryRouteDTO>(() => SelectedDeliveryRoute);
                if (SelectedDeliveryRoute != null)
                {
                    SelectedToAcceptStaff = Staffs.FirstOrDefault(s => s.Id == SelectedDeliveryRoute.AssignedToStaffId);
                    SelectedVehicle = Vehicles.FirstOrDefault(s => s.Id == SelectedDeliveryRoute.VehicleId);
                }
            }
        }
        public TaskProcessDTO SelectedTaskProcess
        {
            get { return _selectedTaskProcess; }
            set
            {
                _selectedTaskProcess = value;
                RaisePropertyChanged<TaskProcessDTO>(() => SelectedTaskProcess);
            }
        }

        #endregion

        #region Commands

        public ICommand SaveDeliveryLineViewCommand
        {
            get { return _saveDeliveryLineViewCommand ?? (_saveDeliveryLineViewCommand = new RelayCommand<Object>(SaveProcess, CanSave)); }
        }
        private void SaveProcess(object obj)
        {
            try
            {
                if (SelectedProcessType == ProcessTypes.Delivery)
                {
                    if (_isAcceptance)
                    {
                        SelectedDeliveryRoute.AssignedToStaffId = SelectedToAcceptStaff.Id;
                        if (SelectedVehicle != null) SelectedDeliveryRoute.VehicleId = SelectedVehicle.Id;
                        SelectedDeliveryRoute.DeliveryMethod = (DeliveryMethods)Convert.ToInt32(SelectedDeliveryMethod.Value);

                        _deliveryService.InsertOrUpdateDeliveryRouteChild(SelectedDeliveryRoute);

                        var delivery = _deliveryService.Find(SelectedDelivery.Id.ToString(CultureInfo.InvariantCulture));
                        delivery.DeliverDirectly = DeliverDirectly;

                        delivery.Status = DeliveryStatusTypes.AcceptanceScheduled;
                        if (SelectedDeliveryRoute.StartedTime != null)
                            delivery.Status = DeliveryStatusTypes.OnAcceptance;

                        if (SelectedDeliveryRoute.StartedTime != null && SelectedDeliveryRoute.EndedTime != null)
                        {
                            delivery.Status = DeliveryStatusTypes.Accepted;
                            if (DeliverDirectly)
                            {
                                MultipleDelivery();
                            }
                        }
                        if (!DeliverDirectly)
                            _deliveryService.InsertOrUpdate(delivery);
                    }
                    else if (!_isAcceptance)
                    {
                        MultipleDelivery();
                    }
                }
                else if (SelectedProcessType == ProcessTypes.DeliveryLine)
                {
                    SelectedDeliveryRoute.AssignedToStaffId = SelectedToAcceptStaff.Id;
                    if (SelectedVehicle != null) SelectedDeliveryRoute.VehicleId = SelectedVehicle.Id;
                    SelectedDeliveryRoute.DeliveryMethod = (DeliveryMethods)Convert.ToInt32(SelectedDeliveryMethod.Value);

                    _deliveryService.InsertOrUpdateDeliveryRouteChild(SelectedDeliveryRoute);

                    var delivery = _deliveryService.Find(SelectedDelivery.Id.ToString(CultureInfo.InvariantCulture));
                    IList<DeliveryRouteDTO> delRoutes = delivery.DeliveryLines.Where(d => d.DeliveryType == DeliveryLineRouteTypes.Delivering)
                                                                .Select(deliveryLineDTO => _deliveryService.GetDeliveryRouteChilds(deliveryLineDTO.Id, false)
                                                                .FirstOrDefault())
                                                                .ToList();
                    var countRoutes = delRoutes.Count;
                    var scheduled = delRoutes.Count(t => t.AssignedToStaffId != null);
                    var ondeliver = delRoutes.Count(t => t.StartedTime != null);
                    var delivered = delRoutes.Count(t => t.StartedTime != null && t.EndedTime != null);

                    if (countRoutes == delivered)
                        delivery.Status = DeliveryStatusTypes.Delivered;
                    else if (ondeliver > 0)
                        delivery.Status = DeliveryStatusTypes.OnDelivery;
                    else if (scheduled > 0)
                        delivery.Status = DeliveryStatusTypes.DeliveryScheduled;

                    _deliveryService.InsertOrUpdate(delivery);
                }
                else if (SelectedProcessType == ProcessTypes.TaskProcess)
                {

                }
                CloseWindow(obj);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't Save" + Environment.NewLine + exception.Message, "Can't Save", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void MultipleDelivery()
        {
            IList<DeliveryRouteDTO> deliveryRoutes = SelectedDelivery.DeliveryLines
                               .Where(d => d.DeliveryType == DeliveryLineRouteTypes.Delivering)
                               .Select(deliveryLineDTO => _deliveryService.GetDeliveryRouteChilds(deliveryLineDTO.Id, false)
                               .FirstOrDefault()).ToList();

            foreach (var deliveryRouteDTO in deliveryRoutes)
            {
                deliveryRouteDTO.AssignedToStaffId = SelectedToAcceptStaff.Id;
                if (SelectedVehicle != null) deliveryRouteDTO.VehicleId = SelectedVehicle.Id;
                deliveryRouteDTO.DeliveryMethod = (DeliveryMethods)Convert.ToInt32(SelectedDeliveryMethod.Value);
                deliveryRouteDTO.StartedTime = SelectedDeliveryRoute.StartedTime;
                deliveryRouteDTO.EndedTime = SelectedDeliveryRoute.EndedTime;

                _deliveryService.InsertOrUpdateDeliveryRouteChild(deliveryRouteDTO);
            }

            var delivery = _deliveryService.Find(SelectedDelivery.Id.ToString(CultureInfo.InvariantCulture));
            delivery.DeliverDirectly = DeliverDirectly;

            delivery.Status = DeliveryStatusTypes.DeliveryScheduled;
            if (SelectedDeliveryRoute.StartedTime != null)
                delivery.Status = DeliveryStatusTypes.OnDelivery;
            if (SelectedDeliveryRoute.StartedTime != null && SelectedDeliveryRoute.EndedTime != null)
                delivery.Status = DeliveryStatusTypes.Delivered;
            _deliveryService.InsertOrUpdate(delivery);
        }

        public ICommand CloseDeliveryLineViewCommand
        {
            get { return _closeDeliveryLineViewCommand ?? (_closeDeliveryLineViewCommand = new RelayCommand<Object>(CloseWindow)); }
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

        #region Staffs
        private ObservableCollection<StaffDTO> _staffs;
        private StaffDTO _selectedToAcceptStaff;

        public void LoadStaffs()
        {
            var criteria = new SearchCriteria<StaffDTO>();

            IEnumerable<StaffDTO> staffsList = new StaffService(true)
                .GetAll(criteria)
                .OrderBy(i => i.Id)
                .ToList();

            Staffs = new ObservableCollection<StaffDTO>(staffsList);
            SelectedToAcceptStaff = Staffs.FirstOrDefault();
        }
        public StaffDTO SelectedToAcceptStaff
        {
            get { return _selectedToAcceptStaff; }
            set
            {
                _selectedToAcceptStaff = value;
                RaisePropertyChanged<StaffDTO>(() => SelectedToAcceptStaff);
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

        #region Vehicles
        private ObservableCollection<VehicleDTO> _vehicles;
        private VehicleDTO _selectedVehicle;
        private string _vehicleVisibility;

        public VehicleDTO SelectedVehicle
        {
            get { return _selectedVehicle; }
            set
            {
                _selectedVehicle = value;
                RaisePropertyChanged<VehicleDTO>(() => SelectedVehicle);
            }
        }
        public ObservableCollection<VehicleDTO> Vehicles
        {
            get { return _vehicles; }
            set
            {
                _vehicles = value;
                RaisePropertyChanged<ObservableCollection<VehicleDTO>>(() => Vehicles);
            }
        }
        public void LoadVehicles()
        {
            VehicleVisibility = "Visible";

            var criteria = new SearchCriteria<VehicleDTO>();
            IEnumerable<VehicleDTO> vehiclesList = new VehicleService(true)
                .GetAll(criteria)
                .OrderBy(i => i.Id)
                .ToList();

            Vehicles = new ObservableCollection<VehicleDTO>(vehiclesList);
            SelectedVehicle = Vehicles.FirstOrDefault();
        }
        public string VehicleVisibility
        {
            get { return _vehicleVisibility; }
            set
            {
                _vehicleVisibility = value;
                RaisePropertyChanged<string>(() => VehicleVisibility);
            }
        }

        #endregion

        #region Delivery Methods
        private List<ListDataItem> _deliveryMethodList;
        private ListDataItem _selectedDeliveryMethod;

        public List<ListDataItem> DeliveryMethodList
        {
            get { return _deliveryMethodList; }
            set
            {
                _deliveryMethodList = value;
                RaisePropertyChanged<List<ListDataItem>>(() => DeliveryMethodList);
            }
        }
        public ListDataItem SelectedDeliveryMethod
        {
            get { return _selectedDeliveryMethod; }
            set
            {
                _selectedDeliveryMethod = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedDeliveryMethod);
            }
        }

        public void LoadDeliveryMethods()
        {
            var deliveryMethods = (List<ListDataItem>)CommonUtility.GetList(typeof(DeliveryMethods));

            if (deliveryMethods != null && deliveryMethods.Count > 1)
            {
                DeliveryMethodList = deliveryMethods.ToList();//.Skip(1)
                SelectedDeliveryMethod = DeliveryMethodList.FirstOrDefault();
            }
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
