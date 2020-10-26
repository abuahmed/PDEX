using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PDEX.Core;
using PDEX.Core.Common;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;
using PDEX.Service;
using PDEX.Service.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PDEX.WPF.Views;

namespace PDEX.WPF.ViewModel
{
    public class TaskProcessViewModel : ViewModelBase
    {
        #region Fields
        private static ITaskProcessService _clientService;
        private IEnumerable<TaskProcessDTO> _clients;
        private ObservableCollection<TaskProcessDTO> _filteredProcesss;
        private ObservableCollection<AddressDTO> _clientAddressDetail;
        private TaskProcessDTO _selectedProcess;
        private ICommand _addNewProcessViewCommand, _saveProcessViewCommand, _deleteProcessViewCommand,
                         _clientAddressViewCommand, _refreshProcessViewCommand;
        private string _searchText, _clientText, _noOfProcesss;
        #endregion

        #region Constructor
        public TaskProcessViewModel()
        {
            Load();
            ProcessText = "Processs";
        }

        public void Load()
        {
            CleanUp();
            _clientService = new TaskProcessService();
            LoadCategories();
            CheckRoles();
            GetLiveProcesss();
        }

        public static void CleanUp()
        {
            if (_clientService != null)
                _clientService.Dispose();
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
                if (ProcessList != null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(SearchText))
                        {
                            Processs = new ObservableCollection<TaskProcessDTO>
                                (ProcessList.Where(c => c.ProcessDetail.ToLower().Contains(value.ToLower())).ToList());
                        }
                        else
                            Processs = new ObservableCollection<TaskProcessDTO>(ProcessList);
                    }
                    catch
                    {
                        MessageBox.Show("Problem searching client");
                        Processs = new ObservableCollection<TaskProcessDTO>(ProcessList);
                    }
                }
            }
        }
        public string ProcessText
        {
            get { return _clientText; }
            set
            {
                _clientText = value;
                RaisePropertyChanged<string>(() => ProcessText);
            }
        }
        public string NoOfProcesss
        {
            get { return _noOfProcesss; }
            set
            {
                _noOfProcesss = value;
                RaisePropertyChanged<string>(() => NoOfProcesss);
            }
        }
        public TaskProcessDTO SelectedProcess
        {
            get { return _selectedProcess; }
            set
            {
                _selectedProcess = value;
                RaisePropertyChanged<TaskProcessDTO>(() => SelectedProcess);
                if (SelectedProcess != null)
                {
                    ProcessAdressDetail = new ObservableCollection<AddressDTO> { SelectedProcess.CompanyAddress };
                    if (SelectedProcess.TaskProcessCategoryId != null)
                        SelectedCategory = Categories.FirstOrDefault(c => c.Id == SelectedProcess.TaskProcessCategoryId);
                }

            }
        }
        public IEnumerable<TaskProcessDTO> ProcessList
        {
            get { return _clients; }
            set
            {
                _clients = value;
                RaisePropertyChanged<IEnumerable<TaskProcessDTO>>(() => ProcessList);
            }
        }
        public ObservableCollection<TaskProcessDTO> Processs
        {
            get { return _filteredProcesss; }
            set
            {
                _filteredProcesss = value;
                RaisePropertyChanged<ObservableCollection<TaskProcessDTO>>(() => Processs);

                if (Processs != null && Processs.Any())
                {
                    SelectedProcess = Processs.FirstOrDefault();
                    NoOfProcesss = Processs.Count.ToString(CultureInfo.InvariantCulture) + " Processs";
                }
                else
                    AddNewProcess();

            }
        }
        public ObservableCollection<AddressDTO> ProcessAdressDetail
        {
            get { return _clientAddressDetail; }
            set
            {
                _clientAddressDetail = value;
                RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => ProcessAdressDetail);
            }
        }
        #endregion

        #region Commands
        public ICommand AddNewProcessViewCommand
        {
            get { return _addNewProcessViewCommand ?? (_addNewProcessViewCommand = new RelayCommand(AddNewProcess)); }
        }
        private void AddNewProcess()
        {
            if (Categories != null) SelectedCategory = Categories.FirstOrDefault();
            SelectedProcess = new TaskProcessDTO
            {
                CompanyAddress = new AddressDTO
                {
                    Country = "Ethiopia",
                    City = "Addis Abeba"
                }
            };
        }

        public ICommand RefreshProcessViewCommand
        {
            get { return _refreshProcessViewCommand ?? (_refreshProcessViewCommand = new RelayCommand(Load)); }
        }

        public ICommand SaveProcessViewCommand
        {
            get { return _saveProcessViewCommand ?? (_saveProcessViewCommand = new RelayCommand<Object>(SaveProcess, CanSave)); }
        }
        private void SaveProcess(object obj)
        {
            if (SelectedProcess == null)
            {
                return;
            }
            try
            {
                if (SelectedProcess.CompanyAddress != null)
                {
                    if (string.IsNullOrEmpty(SelectedProcess.CompanyAddress.Mobile))
                    {
                        MessageBox.Show("Fill Process Mobile Number!!");
                        ProcessAddress();
                        return;
                    }
                }

                var newObject = SelectedProcess.Id;

                SelectedProcess.TaskProcessCategoryId = SelectedCategory.Id;
                var stat = _clientService.InsertOrUpdate(SelectedProcess);
                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (newObject == 0)
                    Processs.Insert(0, SelectedProcess);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                  + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand DeleteProcessViewCommand
        {
            get { return _deleteProcessViewCommand ?? (_deleteProcessViewCommand = new RelayCommand<Object>(DeleteProcess, CanSave)); }
        }
        private void DeleteProcess(object obj)
        {
            if (MessageBox.Show("Are you Sure You want to Delete this Process?", "Delete Process", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedProcess.Enabled = false;
                    var stat = _clientService.Disable(SelectedProcess);
                    if (stat == string.Empty)
                    {
                        Processs.Remove(SelectedProcess);
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

        public ICommand ProcessAddressViewCommand
        {
            get { return _clientAddressViewCommand ?? (_clientAddressViewCommand = new RelayCommand(ProcessAddress)); }
        }
        public void ProcessAddress()
        {
            new AddressEntry(SelectedProcess.CompanyAddress).ShowDialog();
        }

        //public ICommand ChangeProcessCodeViewCommand
        //{
        //    get { return _changeProcessCodeViewCommand ?? (_changeProcessCodeViewCommand = new RelayCommand(ChangeProcessCode)); }
        //}
        //public void ChangeProcessCode()
        //{
        //    if (SelectedProcess != null)
        //        SelectedProcess.Code = CommonUtility.GetSecretCode();
        //}

        #endregion

        public void GetLiveProcesss()
        {
            var criteria = new SearchCriteria<TaskProcessDTO>();
            
            ProcessList = _clientService.GetAll(criteria).OrderBy(i => i.Id).ToList();

            Processs = new ObservableCollection<TaskProcessDTO>(ProcessList);
        }

        #region Categories
        public void LoadCategories()
        {
            var criteria = new SearchCriteria<CategoryDTO>();
            criteria.FiList.Add(c => c.NameType == NameTypes.TaskProcessType);
            IEnumerable<CategoryDTO> categoriesList = new CategoryService(true)
                .GetAll(criteria)
                .ToList();

            Categories = new ObservableCollection<CategoryDTO>(categoriesList);
        }

        private CategoryDTO _selectedCategory;
        public CategoryDTO SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                RaisePropertyChanged<CategoryDTO>(() => SelectedCategory);
            }
        }

        private ObservableCollection<CategoryDTO> _categories;
        public ObservableCollection<CategoryDTO> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                RaisePropertyChanged<ObservableCollection<CategoryDTO>>(() => Categories);
            }
        }

        private ICommand _addNewCategoryCommand;
        public ICommand AddNewCategoryCommand
        {
            get
            {
                return _addNewCategoryCommand ?? (_addNewCategoryCommand = new RelayCommand(ExcuteAddNewCategoryCommand));
            }
        }
        private void ExcuteAddNewCategoryCommand()
        {
            var category = new Categories(NameTypes.TaskProcessType);
            category.ShowDialog();
            var dialogueResult = category.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                LoadCategories();
                SelectedCategory = Categories.FirstOrDefault(c => c.DisplayName == category.TxtCategoryName.Text);
                if (SelectedCategory != null) SelectedProcess.TaskProcessCategoryId = SelectedCategory.Id;
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
            //UserRoles = Singleton.UserRoles;
        }

        #endregion
    }
}
