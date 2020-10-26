using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using PDEX.Core;
using PDEX.Core.Common;
using PDEX.Core.Enumerations;
using PDEX.Core.Extensions;
using PDEX.Core.Models;
using PDEX.Service;
using PDEX.Service.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PDEX.WPF.Views;

namespace PDEX.WPF.ViewModel
{
    public class StaffViewModel : ViewModelBase
    {
        #region Fields
        private static IStaffService _staffService;
        private IEnumerable<StaffDTO> _businessPartners;
        private ObservableCollection<StaffDTO> _filteredStaffs;
        private StaffDTO _selectedStaff;
        private ICommand _addNewStaffViewCommand, _saveStaffViewCommand, _deleteStaffViewCommand,
            _staffAddressViewCommand, _staffContactAddressViewCommand;
        private string _searchText, _staffText;
        #endregion

        #region Constructor
        public StaffViewModel()
        {
            CleanUp();
            _staffService = new StaffService();

            FillStaffTypes();
            CheckRoles();
            GetLiveStaffs();

            StaffText = "Staffs";
        }
        public static void CleanUp()
        {
            if (_staffService != null)
                _staffService.Dispose();
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
                if (StaffList != null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(SearchText))
                        {
                            Staffs = new ObservableCollection<StaffDTO>
                                (StaffList.Where(c => c.StaffDetail.ToLower().Contains(value.ToLower())).ToList());
                        }
                        else
                            Staffs = new ObservableCollection<StaffDTO>(StaffList);
                    }
                    catch
                    {
                        MessageBox.Show("Problem searching staff");
                        Staffs = new ObservableCollection<StaffDTO>(StaffList);
                    }
                }
            }
        }
        public string StaffText
        {
            get { return _staffText; }
            set
            {
                _staffText = value;
                RaisePropertyChanged<string>(() => StaffText);
            }
        }

        public StaffDTO SelectedStaff
        {
            get { return _selectedStaff; }
            set
            {
                _selectedStaff = value;
                RaisePropertyChanged<StaffDTO>(() => SelectedStaff);
                if (SelectedStaff != null)
                {
                    EmployeeShortImage = ImageUtil.ToImage(SelectedStaff.ShortPhoto);// ??
                                         //new BitmapImage(new Uri("../Resources/main.jpg",true));

                    if (StaffTypeList != null)
                        SelectedStaffType = StaffTypeList.FirstOrDefault(s => s.Value == (int)SelectedStaff.Type);

                    StaffAdressDetail = new ObservableCollection<AddressDTO>();
                    StaffAdressDetail.Add(SelectedStaff.Address);

                    ContactAdressDetail = new ObservableCollection<AddressDTO>();
                    if (SelectedStaff.ContactPerson != null && SelectedStaff.ContactPerson.Address != null)
                        ContactAdressDetail.Add(SelectedStaff.ContactPerson.Address);
                }
            }
        }
        public IEnumerable<StaffDTO> StaffList
        {
            get { return _businessPartners; }
            set
            {
                _businessPartners = value;
                RaisePropertyChanged<IEnumerable<StaffDTO>>(() => StaffList);
            }
        }
        public ObservableCollection<StaffDTO> Staffs
        {
            get { return _filteredStaffs; }
            set
            {
                _filteredStaffs = value;
                RaisePropertyChanged<ObservableCollection<StaffDTO>>(() => Staffs);

                if (Staffs != null && Staffs.Any())
                    SelectedStaff = Staffs.FirstOrDefault();
                else
                    AddNewStaff();
            }
        }

        private ObservableCollection<AddressDTO> _staffAddressDetail;
        public ObservableCollection<AddressDTO> StaffAdressDetail
        {
            get { return _staffAddressDetail; }
            set
            {
                _staffAddressDetail = value;
                RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => StaffAdressDetail);
            }
        }

        private ObservableCollection<AddressDTO> _contactAddressDetail;
        public ObservableCollection<AddressDTO> ContactAdressDetail
        {
            get { return _contactAddressDetail; }
            set
            {
                _contactAddressDetail = value;
                RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => ContactAdressDetail);
            }
        }
        #endregion

        #region Filter List
        private List<ListDataItem> _staffTypeList;
        private ListDataItem _selectedStaffType;

        public List<ListDataItem> StaffTypeList
        {
            get { return _staffTypeList; }
            set
            {
                _staffTypeList = value;
                RaisePropertyChanged<List<ListDataItem>>(() => StaffTypeList);
            }
        }
        public ListDataItem SelectedStaffType
        {
            get { return _selectedStaffType; }
            set
            {
                _selectedStaffType = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedStaffType);
            }
        }

        private List<ListDataItem> _staffTypeListForFilter;
        private ListDataItem _selectedStaffTypeForFilter;

        public List<ListDataItem> StaffTypeListForFilter
        {
            get { return _staffTypeListForFilter; }
            set
            {
                _staffTypeListForFilter = value;
                RaisePropertyChanged<List<ListDataItem>>(() => StaffTypeListForFilter);
            }
        }
        public ListDataItem SelectedStaffTypeForFilter
        {
            get { return _selectedStaffTypeForFilter; }
            set
            {
                _selectedStaffTypeForFilter = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedStaffTypeForFilter);
                GetLiveStaffs();
            }
        }

        public void FillStaffTypes()
        {
            var staffTypes = (List<ListDataItem>)CommonUtility.GetList(typeof(StaffTypes));

            StaffTypeListForFilter = staffTypes.ToList();
            SelectedStaffTypeForFilter = StaffTypeListForFilter.FirstOrDefault();

            if (staffTypes != null && staffTypes.Count > 1)
            {
                StaffTypeList = staffTypes.Skip(1).ToList();
                SelectedStaffType = StaffTypeList.FirstOrDefault();
            }
        }


        #endregion

        #region Commands
        public ICommand AddNewStaffViewCommand
        {
            get { return _addNewStaffViewCommand ?? (_addNewStaffViewCommand = new RelayCommand(AddNewStaff)); }
        }
        private void AddNewStaff()
        {
            try
            {
                SelectedStaff = new StaffDTO
                {
                    Code = _staffService.GetStaffCode(),
                    Type = StaffTypes.OfficeStaff,
                    EducationLevel = EducationLevelTypes.Elementary,
                    IsActive = true,
                    Sex = Sex.Male,
                    Address = new AddressDTO
                    {
                        Country = "Ethiopia",
                        City = "Addis Abeba"
                    },
                };

                SelectedStaff.ContactPerson = new ContactPersonDTO()
                {
                    DisplayName = "-",
                    Address = new AddressDTO
                    {
                        Country = "Ethiopia",
                        City = "Addis Abeba"
                    }
                };

                EmployeeShortImage = new BitmapImage();
            }
            catch (Exception)
            {
                //MessageBox.Show("Problem on adding new staff");
            }
        }

        public ICommand SaveStaffViewCommand
        {
            get { return _saveStaffViewCommand ?? (_saveStaffViewCommand = new RelayCommand<Object>(SaveStaff, CanSave)); }
        }
        private void SaveStaff(object obj)
        {
            try
            {
                if (EmployeeShortImage != null && EmployeeShortImage.UriSource != null)
                    SelectedStaff.ShortPhoto = ImageUtil.ToBytes(EmployeeShortImage);

                var newObject = SelectedStaff.Id;
                SelectedStaff.Type = (StaffTypes)SelectedStaffType.Value;

                var stat = _staffService.InsertOrUpdate(SelectedStaff);

                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (newObject == 0)
                    Staffs.Insert(0, SelectedStaff);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                  + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand DeleteStaffViewCommand
        {
            get { return _deleteStaffViewCommand ?? (_deleteStaffViewCommand = new RelayCommand<Object>(DeleteStaff, CanSave)); }
        }
        private void DeleteStaff(object obj)
        {
            if (MessageBox.Show("Are you Sure You want to Delete this Staff?", "Delete Staff", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedStaff.Enabled = false;
                    var stat = _staffService.Disable(SelectedStaff);
                    if (stat == string.Empty)
                    {
                        Staffs.Remove(SelectedStaff);
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

        public ICommand StaffAddressViewCommand
        {
            get { return _staffAddressViewCommand ?? (_staffAddressViewCommand = new RelayCommand(StaffAddress)); }
        }
        public void StaffAddress()
        {
            new AddressEntry(SelectedStaff.Address).ShowDialog();
        }

        public ICommand StaffContactAddressViewCommand
        {
            get { return _staffContactAddressViewCommand ?? (_staffContactAddressViewCommand = new RelayCommand(StaffContactAddress)); }
        }
        public void StaffContactAddress()
        {
            if (SelectedStaff != null && SelectedStaff.ContactPerson != null && SelectedStaff.ContactPerson.Address != null)
                new AddressEntry(SelectedStaff.ContactPerson.Address).ShowDialog();
        }

        #endregion

        public void GetLiveStaffs()
        {
            var criteria = new SearchCriteria<StaffDTO>();

            var stafType = (StaffTypes)SelectedStaffTypeForFilter.Value;
            if (stafType != StaffTypes.All)
                criteria.FiList.Add(b => b.Type == stafType);

            StaffList = _staffService.GetAll(criteria).OrderBy(i => i.Id).ToList();

            Staffs = new ObservableCollection<StaffDTO>(StaffList);
        }

        #region Short Photo
        private BitmapImage _employeeShortImage;
        private ICommand _showEmployeeShortImageCommand;

        public BitmapImage EmployeeShortImage
        {
            get { return _employeeShortImage; }
            set
            {
                _employeeShortImage = value;
                RaisePropertyChanged<BitmapImage>(() => EmployeeShortImage);
                
            }
        }
        public ICommand ShowEmployeeShortImageCommand
        {
            get
            {
                return _showEmployeeShortImageCommand ??
                       (_showEmployeeShortImageCommand = new RelayCommand(ExecuteShowEmployeeShortImageViewCommand));
            }
        }
        private void ExecuteShowEmployeeShortImageViewCommand()
        {
            var file = new OpenFileDialog { Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg" };
            var result = file.ShowDialog();
            if (result != null && ((bool)result && File.Exists(file.FileName)))
            {
                EmployeeShortImage = new BitmapImage(new Uri(file.FileName, true));
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
