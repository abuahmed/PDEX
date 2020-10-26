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

namespace PDEX.WPF.ViewModel
{
    public class AcceptanceEntryViewModel : ViewModelBase
    {
        #region Fields
        private CompanyDTO _selectedCompany;
        private DeliveryHeaderDTO _selectedDelivery;
        private DeliveryLineDTO _selectedDeliveryLine;
        private DeliveryRouteDTO _selectedDeliveryRoute;
        private static IDeliveryService _deliveryService;
        private bool _deliverDirectly;
        private ICommand _saveDeliveryLineViewCommand, _closeDeliveryLineViewCommand, _resetDeliveryLineViewCommand;
        #endregion

        #region Constructor
        public AcceptanceEntryViewModel()
        {
            CleanUp();
            _deliveryService = new DeliveryService();

            LoadStaffs();
            LoadVehicles();
            CheckRoles();

            SelectedCompany = new CompanyService(true).GetCompany();

            Messenger.Default.Register<DeliveryHeaderDTO>(this, (message) =>
            {
                SelectedDelivery = message;
            });

        }
        public static void CleanUp()
        {
            if (_deliveryService != null)
                _deliveryService.Dispose();
        }
        #endregion

        #region Public Properties

        public CompanyDTO SelectedCompany
        {
            get { return _selectedCompany; }
            set
            {
                _selectedCompany = value;
                RaisePropertyChanged<CompanyDTO>(() => SelectedCompany);

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
                    SelectedDeliveryLine = SelectedDelivery.DeliveryLines.FirstOrDefault(d => d.DeliveryType == DeliveryLineRouteTypes.Accepting);
                    
                    if (SelectedDeliveryLine != null)
                    {
                        SelectedDeliveryRoute = _deliveryService.GetDeliveryRouteChilds(SelectedDeliveryLine.Id, false).FirstOrDefault() ??
                                                new DeliveryRouteDTO()
                                                {
                                                    DeliveryLineId = SelectedDeliveryLine.Id
                                                };
                    }
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
                    SelectedStaff = Staffs.FirstOrDefault(s => s.Id == SelectedDeliveryRoute.AssignedToStaffId);
                    SelectedVehicle = Vehicles.FirstOrDefault(s => s.Id == SelectedDeliveryRoute.VehicleId);
                }
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
                SelectedDeliveryRoute.AssignedToStaffId = SelectedStaff.Id;
                if (SelectedVehicle != null) SelectedDeliveryRoute.VehicleId = SelectedVehicle.Id;

                SelectedDeliveryRoute.FromAddressId = SelectedCompany.AddressId;
                SelectedDeliveryRoute.ToAddressId = SelectedDelivery.OrderByClient.AddressId;
                _deliveryService.InsertOrUpdateDeliveryRouteChild(SelectedDeliveryRoute);

                var delivery = _deliveryService.Find(SelectedDelivery.Id.ToString());

                delivery.DeliverDirectly = DeliverDirectly;

                delivery.Status = DeliveryStatusTypes.AcceptanceScheduled;
                if (SelectedDeliveryRoute.StartedTime != null)
                    delivery.Status = DeliveryStatusTypes.OnAcceptance;
                if (SelectedDeliveryRoute.StartedTime != null && SelectedDeliveryRoute.EndedTime != null)
                    delivery.Status = DeliveryStatusTypes.Accepted;
                _deliveryService.InsertOrUpdate(delivery);

                //Create Delivery Route for each Line
                var delLines = SelectedDelivery.DeliveryLines.Where(
                    d => d.DeliveryType == DeliveryLineRouteTypes.Delivering).ToList();
                foreach (var deliveryLineDTO in delLines)
                {
                    var delRoute =
                        _deliveryService.GetDeliveryRouteChilds(deliveryLineDTO.Id, false).FirstOrDefault();
                    
                    if (DeliverDirectly && delRoute == null)
                    {
                        delRoute = new DeliveryRouteDTO();
                        delRoute = (DeliveryRouteDTO)MapperUtility<DeliveryRouteDTO>.GetMap(SelectedDeliveryRoute, delRoute);
                        delRoute.DeliveryType = DeliveryLineRouteTypes.Delivering;
                        delRoute.DeliveryLine = null;
                        delRoute.DeliveryLineId = deliveryLineDTO.Id;
                        delRoute.FromAddressId = SelectedDelivery.OrderByClient.AddressId;
                        delRoute.StartedTime = null;
                        delRoute.EndedTime = null;
                        delRoute.Id = 0;

                        _deliveryService.InsertOrUpdateDeliveryRouteChild(delRoute);

                    }
                    if (delRoute != null && !DeliverDirectly )
                    {
                        delRoute.Enabled = false;
                        _deliveryService.InsertOrUpdateDeliveryRouteChild(delRoute);
                    }
                }

                CloseWindow(obj);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                  + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand ResetDeliveryLineViewCommand
        {
            get { return _resetDeliveryLineViewCommand ?? (_resetDeliveryLineViewCommand = new RelayCommand(ResetDeliveryLine)); }
        }
        private void ResetDeliveryLine()
        {
            SelectedDeliveryRoute = new DeliveryRouteDTO
            {

            };
        }

        public ICommand CloseDeliveryLineViewCommand
        {
            get { return _closeDeliveryLineViewCommand ?? (_closeDeliveryLineViewCommand = new RelayCommand<Object>(CloseWindow)); }
        }
        private void CloseWindow(object obj)
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
