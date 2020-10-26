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
    public class DeliveryEntryViewModel : ViewModelBase
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
        public DeliveryEntryViewModel()
        {
            CleanUp();
            _deliveryService = new DeliveryService();

            LoadStaffs();
            LoadVehicles();
            CheckRoles();
            SelectedCompany = new CompanyService(true).GetCompany();

            Messenger.Default.Register<DeliveryLineDTO>(this, (message) =>
            {
                SelectedDeliveryLine = message;
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
                    SelectedDelivery = SelectedDeliveryLine.DeliveryHeader;
                    SelectedDeliveryRoute =
                        _deliveryService.GetDeliveryRouteChilds(SelectedDeliveryLine.Id, false).FirstOrDefault();

                    if (SelectedDeliveryRoute == null)
                    {
                        var selectedLine = SelectedDelivery.DeliveryLines.FirstOrDefault(d => d.DeliveryType == DeliveryLineRouteTypes.Accepting);
                        if (selectedLine != null)
                        {
                            var selectedDelRoute = _deliveryService.GetDeliveryRouteChilds(selectedLine.Id, false).FirstOrDefault();

                            var delRoute = new DeliveryRouteDTO();
                            delRoute = (DeliveryRouteDTO)MapperUtility<DeliveryRouteDTO>.GetMap(selectedDelRoute, delRoute) ??
                                       new DeliveryRouteDTO();
                            delRoute.DeliveryType = DeliveryLineRouteTypes.Delivering;
                            delRoute.DeliveryLine = null;
                            delRoute.DeliveryLineId = SelectedDeliveryLine.Id;
                            delRoute.FromAddressId = SelectedCompany.AddressId;
                            delRoute.StartedTime = null;
                            delRoute.EndedTime = null;
                            delRoute.Id = 0;

                            SelectedDeliveryRoute = delRoute;
                        }
                    }

                }
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
                var delivery = _deliveryService.Find(SelectedDelivery.Id.ToString(CultureInfo.InvariantCulture));

                SelectedDeliveryRoute.AssignedToStaffId = SelectedStaff.Id;
                if (SelectedVehicle != null)
                    SelectedDeliveryRoute.VehicleId = SelectedVehicle.Id;

                SelectedDeliveryRoute.FromAddressId = delivery.DeliverDirectly ?
                                                        SelectedDelivery.OrderByClient.AddressId :
                                                        SelectedCompany.AddressId;

                SelectedDeliveryRoute.ToAddressId = SelectedDeliveryLine.ToClient.AddressId;

                _deliveryService.InsertOrUpdateDeliveryRouteChild(SelectedDeliveryRoute);

                IList<DeliveryRouteDTO> tempRoutes = delivery.DeliveryLines
                    .Select(deliveryLineDTO => _deliveryService.GetDeliveryRouteChilds(deliveryLineDTO.Id, false)
                        .FirstOrDefault()).ToList();

                var countRoutes = tempRoutes.Count;
                var scheduled = tempRoutes.Count(t => t.AssignedToStaffId != null);
                var ondeliver = tempRoutes.Count(t => t.StartedTime != null);
                var delivered = tempRoutes.Count(t => t.StartedTime != null && t.EndedTime != null);

                if (countRoutes == delivered)
                    delivery.Status = DeliveryStatusTypes.Delivered;
                else if (ondeliver > 0)
                    delivery.Status = DeliveryStatusTypes.OnDelivery;
                else if (scheduled > 0)
                    delivery.Status = DeliveryStatusTypes.DeliveryScheduled;

                _deliveryService.InsertOrUpdate(delivery);

                CloseWindow(obj);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save route"
                                  + Environment.NewLine + exception.Message, "Can't save route", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand ResetDeliveryLineViewCommand
        {
            get { return _resetDeliveryLineViewCommand ?? (_resetDeliveryLineViewCommand = new RelayCommand(ResetDeliveryLine)); }
        }
        public void ResetDeliveryLine()
        {

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
