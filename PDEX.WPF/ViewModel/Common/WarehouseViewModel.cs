using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PDEX.Core;
using PDEX.Core.Common;
using PDEX.Core.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PDEX.DAL;
using PDEX.Repository;
using PDEX.Service;
using PDEX.Service.Interfaces;
using MessageBox = System.Windows.MessageBox;

namespace PDEX.WPF.ViewModel
{
    public class WarehouseViewModel : ViewModelBase
    {
        #region Fields
        private static IWarehouseService _warehouseService;
        private ObservableCollection<WarehouseDTO> _filteredWarehouses;
        private WarehouseDTO _selectedWarehouse;
        private ICommand _addNewWarehouseCommand, _saveWarehouseCommand, _deleteWarehouseCommand;

        #endregion

        #region Constructor
        public WarehouseViewModel()
        {
            CleanUp();
            _warehouseService = new WarehouseService();
            CheckRoles();
            GetLiveWarehouses();
        }

        public static void CleanUp()
        {
            if (_warehouseService != null)
                _warehouseService.Dispose();
        }
        #endregion

        #region Properties
        public ObservableCollection<WarehouseDTO> Warehouses
        {
            get { return _filteredWarehouses; }
            set
            {
                _filteredWarehouses = value;
                RaisePropertyChanged<ObservableCollection<WarehouseDTO>>(() => Warehouses);
                if (Warehouses.Any())
                    SelectedWarehouse = Warehouses.FirstOrDefault();
                else
                    ExecuteAddNewWarehouseViewCommand();
            }
        }

        public WarehouseDTO SelectedWarehouse
        {
            get { return _selectedWarehouse; }
            set
            {
                _selectedWarehouse = value;
                RaisePropertyChanged<WarehouseDTO>(() => SelectedWarehouse);
                if (SelectedWarehouse != null)
                {
                }

            }
        }


        public void GetLiveWarehouses()
        {
            var criteria = new SearchCriteria<WarehouseDTO>();

            var warehousesList = _warehouseService.GetAll(criteria);
            Warehouses = new ObservableCollection<WarehouseDTO>(warehousesList);
        }
        #endregion

        #region Commands
        public ICommand AddNewWarehouseViewCommand
        {
            get { return _addNewWarehouseCommand ?? (_addNewWarehouseCommand = new RelayCommand(ExecuteAddNewWarehouseViewCommand)); }
        }
        private void ExecuteAddNewWarehouseViewCommand()
        {
            SelectedWarehouse = new WarehouseDTO
            {
                Address = new AddressDTO
                {
                    Country = "Ethiopia",
                    City = "Addis Abeba"
                }
            };
        }

        public ICommand SaveWarehouseViewCommand
        {
            get { return _saveWarehouseCommand ?? (_saveWarehouseCommand = new RelayCommand<Object>(ExecuteSaveWarehouseViewCommand, CanSave)); }
        }
        private void ExecuteSaveWarehouseViewCommand(object obj)
        {
            try
            {
                var wareId = SelectedWarehouse.Id;
                var stat = _warehouseService.InsertOrUpdate(SelectedWarehouse);
                if (stat == string.Empty)
                {
                    #region Storage Bins
                    if (true)//wareId == 0
                    {
                        var iDbContext = DbContextUtil.GetDbContextInstance();
                        try
                        {
                            var unitOfWork = new UnitOfWork(iDbContext);
                            var storageBinRepository = unitOfWork.Repository<StorageBinDTO>();

                            var storageBins = storageBinRepository.Query()
                                .Filter(sb => sb.WarehouseId == SelectedWarehouse.Id)
                                .Get()
                                .ToList();

                            foreach (var storageBinDTO in storageBins)
                            {
                                storageBinRepository.Delete(storageBinDTO.Id);
                            }

                            for (var i = 1; i <= SelectedWarehouse.NoOfShelves; i++)
                            {
                                for (var j = 1; j <= SelectedWarehouse.NoOfBoxes; j++)
                                {
                                    storageBinRepository.Insert(new StorageBinDTO
                                    {
                                        Shelve = i.ToString(CultureInfo.InvariantCulture),
                                        BoxNumber = j.ToString(CultureInfo.InvariantCulture),
                                        WarehouseId = SelectedWarehouse.Id,
                                        IsActive = true
                                    });
                                }
                            }
                            unitOfWork.Commit();
                        }
                        catch
                        {
                            MessageBox.Show("Got Problem saving Storage Bins");
                        }
                        finally
                        {
                            iDbContext.Dispose();
                        }
                    }

                    #endregion

                    CloseWindow(obj);
                }
                else
                    MessageBox.Show("Got Problem while saving, try again..." + Environment.NewLine + stat, "save error", MessageBoxButton.OK,
                       MessageBoxImage.Error);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Got Problem while saving, try again..." + Environment.NewLine + exception.Message, "save error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        public ICommand DeleteWarehouseViewCommand
        {
            get { return _deleteWarehouseCommand ?? (_deleteWarehouseCommand = new RelayCommand(ExecuteDeleteWarehouseViewCommand, CanSaveLine)); }
        }
        private void ExecuteDeleteWarehouseViewCommand()
        {
            if (MessageBox.Show("Are you Sure You want to Delete this Warehouse?", "Delete Warehouse",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {

                try
                {
                    SelectedWarehouse.Enabled = false;
                    _warehouseService.Disable(SelectedWarehouse);
                    GetLiveWarehouses();
                }
                catch (Exception)
                {
                    MessageBox.Show("Can't delete the warehouse, may be the warehouse is already in use...", "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        public void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window != null)
            {
                window.Close();
            }
        }
        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object obj)
        {
            return Errors == 0;

        }

        public static int LineErrors { get; set; }
        public bool CanSaveLine()
        {
            return LineErrors == 0;

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
