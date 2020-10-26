using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PDEX.Core;
using PDEX.Core.Common;
using PDEX.Core.Enumerations;
using PDEX.Core.Extensions;
using PDEX.Core.Models;
using PDEX.Service;
using PDEX.Service.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PDEX.WPF.ViewModel
{
    public class VehicleViewModel : ViewModelBase
    {
        #region Fields
        private static IVehicleService _vehicleService;
        private IEnumerable<VehicleDTO> _businessPartners;
        private ObservableCollection<VehicleDTO> _filteredVehicles;
        private VehicleDTO _selectedVehicle;
        private ICommand _addNewVehicleViewCommand, _saveVehicleViewCommand, _deleteVehicleViewCommand;
        private string _searchText, _vehicleText;
        #endregion

        #region Constructor
        public VehicleViewModel()
        {
            CleanUp();
            _vehicleService = new VehicleService();

            FillVehicleTypes();
            LoadDrivers();
            CheckRoles();
            GetLiveVehicles();

            VehicleText = "Vehicles";
        }
        public static void CleanUp()
        {
            if (_vehicleService != null)
                _vehicleService.Dispose();
        }
        #endregion

        #region Public Properties
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged<string>(() => SearchText);
                if (VehicleList != null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(SearchText))
                        {
                            Vehicles = new ObservableCollection<VehicleDTO>
                                (VehicleList.Where(c => c.VehicleDetail.ToLower().Contains(value.ToLower())).ToList());
                        }
                        else
                            Vehicles = new ObservableCollection<VehicleDTO>(VehicleList);
                    }
                    catch
                    {
                        MessageBox.Show("Problem searching vehicle");
                        Vehicles = new ObservableCollection<VehicleDTO>(VehicleList);
                    }
                }
            }
        }
        public string VehicleText
        {
            get { return _vehicleText; }
            set
            {
                _vehicleText = value;
                RaisePropertyChanged<string>(() => VehicleText);
            }
        }

        public VehicleDTO SelectedVehicle
        {
            get { return _selectedVehicle; }
            set
            {
                _selectedVehicle = value;
                RaisePropertyChanged<VehicleDTO>(() => SelectedVehicle);
                if (SelectedVehicle != null)
                {
                    if (SelectedVehicle.AssignedDriverId != null && SelectedVehicle.AssignedDriverId != 0)
                        SelectedDriver = Drivers.FirstOrDefault(d => d.Id == SelectedVehicle.AssignedDriverId);
                    else
                        SelectedDriver = null;


                    if (VehicleTypeList != null)
                        SelectedVehicleType = VehicleTypeList.FirstOrDefault(s => s.Value == (int)SelectedVehicle.Type);
                }
            }
        }
        public IEnumerable<VehicleDTO> VehicleList
        {
            get { return _businessPartners; }
            set
            {
                _businessPartners = value;
                RaisePropertyChanged<IEnumerable<VehicleDTO>>(() => VehicleList);
            }
        }
        public ObservableCollection<VehicleDTO> Vehicles
        {
            get { return _filteredVehicles; }
            set
            {
                _filteredVehicles = value;
                RaisePropertyChanged<ObservableCollection<VehicleDTO>>(() => Vehicles);

                if (Vehicles != null && Vehicles.Any())
                    SelectedVehicle = Vehicles.FirstOrDefault();
                else
                    AddNewVehicle();
            }
        }
        #endregion

        #region Filter List
        private List<ListDataItem> _vehicleTypeList;
        private ListDataItem _selectedVehicleType;

        public List<ListDataItem> VehicleTypeList
        {
            get { return _vehicleTypeList; }
            set
            {
                _vehicleTypeList = value;
                RaisePropertyChanged<List<ListDataItem>>(() => VehicleTypeList);
            }
        }
        public ListDataItem SelectedVehicleType
        {
            get { return _selectedVehicleType; }
            set
            {
                _selectedVehicleType = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedVehicleType);
            }
        }

        private List<ListDataItem> _vehicleTypeListForFilter;
        private ListDataItem _selectedVehicleTypeForFilter;

        public List<ListDataItem> VehicleTypeListForFilter
        {
            get { return _vehicleTypeListForFilter; }
            set
            {
                _vehicleTypeListForFilter = value;
                RaisePropertyChanged<List<ListDataItem>>(() => VehicleTypeListForFilter);
            }
        }
        public ListDataItem SelectedVehicleTypeForFilter
        {
            get { return _selectedVehicleTypeForFilter; }
            set
            {
                _selectedVehicleTypeForFilter = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedVehicleTypeForFilter);
                GetLiveVehicles();
            }
        }

        public void FillVehicleTypes()
        {
            var vehicleTypes = (List<ListDataItem>)CommonUtility.GetList(typeof(VehicleTypes));

            if (vehicleTypes != null && vehicleTypes.Count > 1)
                VehicleTypeList = vehicleTypes.Skip(1).ToList();

            //VehicleTypeListForFilter = vehicleTypes.ToList();
            //SelectedVehicleTypeForFilter = VehicleTypeListForFilter.FirstOrDefault();
        }


        #endregion

        #region Commands
        public ICommand AddNewVehicleViewCommand
        {
            get { return _addNewVehicleViewCommand ?? (_addNewVehicleViewCommand = new RelayCommand(AddNewVehicle)); }
        }
        private void AddNewVehicle()
        {
            SelectedDriver = null;
            SelectedVehicle = new VehicleDTO
            {
                Type = (VehicleTypes)VehicleTypeList.FirstOrDefault().Value,
                Number = _vehicleService.GetVehicleCode()
            };
        }

        public ICommand SaveVehicleViewCommand
        {
            get { return _saveVehicleViewCommand ?? (_saveVehicleViewCommand = new RelayCommand<Object>(SaveVehicle, CanSave)); }
        }
        private void SaveVehicle(object obj)
        {
            try
            {
                if (SelectedDriver != null)
                    SelectedVehicle.AssignedDriverId = SelectedDriver.Id;

                var newObject = SelectedVehicle.Id;
                SelectedVehicle.Type = (VehicleTypes)SelectedVehicleType.Value;

                var stat = _vehicleService.InsertOrUpdate(SelectedVehicle);
                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (newObject == 0)
                    Vehicles.Insert(0, SelectedVehicle);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                  + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand DeleteVehicleViewCommand
        {
            get { return _deleteVehicleViewCommand ?? (_deleteVehicleViewCommand = new RelayCommand<Object>(DeleteVehicle, CanSave)); }
        }
        private void DeleteVehicle(object obj)
        {
            if (MessageBox.Show("Are you Sure You want to Delete this Vehicle?", "Delete Vehicle", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedVehicle.Enabled = false;
                    var stat = _vehicleService.Disable(SelectedVehicle);
                    if (stat == string.Empty)
                    {
                        Vehicles.Remove(SelectedVehicle);
                    }
                    else
                    {
                        MessageBox.Show("Can't Delete, may be the data is already in use..."
                             + Environment.NewLine + stat, "Can't Delete",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't Delete, may be the data is already in use..."
                         + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        #endregion

        public void GetLiveVehicles()
        {
            var criteria = new SearchCriteria<VehicleDTO>();

            //var stafType = (VehicleTypes)SelectedVehicleTypeForFilter.Value;
            //if (stafType != VehicleTypes.All)
            //    criteria.FiList.Add(b => b.Type == stafType);

            VehicleList = _vehicleService.GetAll(criteria).OrderBy(i => i.Id).ToList();

            Vehicles = new ObservableCollection<VehicleDTO>(VehicleList);
        }

        #region Assigned Drivers
        private ObservableCollection<StaffDTO> _drivers;
        public ObservableCollection<StaffDTO> Drivers
        {
            get { return _drivers; }
            set
            {
                _drivers = value;
                RaisePropertyChanged<ObservableCollection<StaffDTO>>(() => Drivers);
            }
        }

        private StaffDTO _selectedDriver;
        public StaffDTO SelectedDriver
        {
            get { return _selectedDriver; }
            set
            {
                _selectedDriver = value;
                RaisePropertyChanged<StaffDTO>(() => SelectedDriver);
            }
        }

        public void LoadDrivers()
        {
            var criteria = new SearchCriteria<StaffDTO>();
            criteria.FiList.Add(s => s.Type == StaffTypes.Driver || s.Type == StaffTypes.Multi);
            //criteria.FiList.Add(s=>s.AssignedVehicles.Count==0);
            //Can driver assigned on more than one vehicle

            var drivers = new StaffService(true).GetAll(criteria)
                .Where(a => a.AssignedVehicles.Count == 0).ToList();

            Drivers = new ObservableCollection<StaffDTO>(drivers);
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
