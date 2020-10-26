using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class ClientViewModel : ViewModelBase
    {
        #region Fields
        private static IClientService _clientService;
        private IEnumerable<ClientDTO> _clients;
        private ObservableCollection<ClientDTO> _filteredClients;
        private ObservableCollection<AddressDTO> _clientAddressDetail;
        private ClientDTO _selectedClient;
        private ICommand _addNewClientViewCommand, _saveClientViewCommand, _deleteClientViewCommand,
                         _clientAddressViewCommand, _changeClientCodeViewCommand, _documentsViewCommand, _refreshClientViewCommand;
        private string _searchText, _clientText, _noOfClients;
        #endregion

        #region Constructor
        public ClientViewModel()
        {
            Load();
            ClientText = "Clients";
        }

        public void Load()
        {
            CleanUp();
            _clientService = new ClientService();

            LoadCategories();
            CheckRoles();
            GetLiveClients();
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
                if (ClientList != null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(SearchText))
                        {
                            Clients = new ObservableCollection<ClientDTO>
                                (ClientList.Where(c => c.ClientDetail.ToLower().Contains(value.ToLower())).ToList());
                        }
                        else
                            Clients = new ObservableCollection<ClientDTO>(ClientList);
                    }
                    catch
                    {
                        MessageBox.Show("Problem searching client");
                        Clients = new ObservableCollection<ClientDTO>(ClientList);
                    }
                }
            }
        }
        public string ClientText
        {
            get { return _clientText; }
            set
            {
                _clientText = value;
                RaisePropertyChanged<string>(() => ClientText);
            }
        }
        public string NoOfClients
        {
            get { return _noOfClients; }
            set
            {
                _noOfClients = value;
                RaisePropertyChanged<string>(() => NoOfClients);
            }
        }
        public ClientDTO SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                RaisePropertyChanged<ClientDTO>(() => SelectedClient);
                if (SelectedClient != null)
                {
                    ClientAdressDetail = new ObservableCollection<AddressDTO> { SelectedClient.Address };
                    if (SelectedClient.CategoryId != null)
                        SelectedCategory = Categories.FirstOrDefault(c => c.Id == SelectedClient.CategoryId);
                }

            }
        }
        public IEnumerable<ClientDTO> ClientList
        {
            get { return _clients; }
            set
            {
                _clients = value;
                RaisePropertyChanged<IEnumerable<ClientDTO>>(() => ClientList);
            }
        }
        public ObservableCollection<ClientDTO> Clients
        {
            get { return _filteredClients; }
            set
            {
                _filteredClients = value;
                RaisePropertyChanged<ObservableCollection<ClientDTO>>(() => Clients);

                if (Clients != null && Clients.Any())
                {
                    SelectedClient = Clients.FirstOrDefault();
                    NoOfClients = Clients.Count.ToString() + " Clients";
                }
                else
                    AddNewClient();

            }
        }
        public ObservableCollection<AddressDTO> ClientAdressDetail
        {
            get { return _clientAddressDetail; }
            set
            {
                _clientAddressDetail = value;
                RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => ClientAdressDetail);
            }
        }
        #endregion

        #region Commands
        public ICommand AddNewClientViewCommand
        {
            get { return _addNewClientViewCommand ?? (_addNewClientViewCommand = new RelayCommand(AddNewClient)); }
        }
        private void AddNewClient()
        {
            if (Categories != null) SelectedCategory = Categories.FirstOrDefault();
            SelectedClient = new ClientDTO
            {
                Number = _clientService.GetClientCode(),
                Code = CommonUtility.GetSecretCode(),
                IsActive = true,
                Address = new AddressDTO
                {
                    Country = "Ethiopia",
                    City = "Addis Abeba"
                }
            };
        }

        public ICommand RefreshClientViewCommand
        {
            get { return _refreshClientViewCommand ?? (_refreshClientViewCommand = new RelayCommand(Load)); }
        }

        public ICommand SaveClientViewCommand
        {
            get { return _saveClientViewCommand ?? (_saveClientViewCommand = new RelayCommand<Object>(SaveClient, CanSave)); }
        }
        private void SaveClient(object obj)
        {
            if (SelectedClient == null)
            {
                return;
            }
            try
            {
                if (SelectedClient.Address != null)
                {
                    if (string.IsNullOrEmpty(SelectedClient.Address.Mobile))
                    {
                        MessageBox.Show("Fill Client Mobile Number!!");
                        ClientAddress();
                        return;
                    }
                }

                var newObject = SelectedClient.Id;

                SelectedClient.CategoryId = SelectedCategory.Id;
                var stat = _clientService.InsertOrUpdate(SelectedClient);
                if (stat != string.Empty)
                    MessageBox.Show("Can't save"
                                    + Environment.NewLine + stat, "Can't save", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else if (newObject == 0)
                    Clients.Insert(0, SelectedClient);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                  + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand DeleteClientViewCommand
        {
            get { return _deleteClientViewCommand ?? (_deleteClientViewCommand = new RelayCommand<Object>(DeleteClient, CanSave)); }
        }
        private void DeleteClient(object obj)
        {
            if (MessageBox.Show("Are you Sure You want to Delete this Client?", "Delete Client", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    SelectedClient.Enabled = false;
                    var stat = _clientService.Disable(SelectedClient);
                    if (stat == string.Empty)
                    {
                        Clients.Remove(SelectedClient);
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

        public ICommand ClientAddressViewCommand
        {
            get { return _clientAddressViewCommand ?? (_clientAddressViewCommand = new RelayCommand(ClientAddress)); }
        }
        public void ClientAddress()
        {
            new AddressEntry(SelectedClient.Address).ShowDialog();
        }

        public ICommand ChangeClientCodeViewCommand
        {
            get { return _changeClientCodeViewCommand ?? (_changeClientCodeViewCommand = new RelayCommand(ChangeClientCode)); }
        }
        public void ChangeClientCode()
        {
            if (SelectedClient != null)
                SelectedClient.Code = CommonUtility.GetSecretCode();
        }

        public ICommand DocumentsViewCommand
        {
            get { return _documentsViewCommand ?? (_documentsViewCommand = new RelayCommand(ViewDocuments)); }
        }

        public void ViewDocuments()
        {
            new Documents(SelectedClient).ShowDialog();
        }

        #endregion

        public void GetLiveClients()
        {
            var criteria = new SearchCriteria<ClientDTO>();
            criteria.FiList.Add(b => b.IsReceiver == false);

            ClientList = _clientService.GetAll(criteria).OrderBy(i => i.Id).ToList();

            Clients = new ObservableCollection<ClientDTO>(ClientList);

            var sno = 1;
            foreach (var clientDTO in Clients)
            {
                clientDTO.SerialNumber = sno;
                sno++;
            }
        }

        #region Categories
        public void LoadCategories()
        {
            var criteria = new SearchCriteria<CategoryDTO>();
            criteria.FiList.Add(c => c.NameType == NameTypes.ClientCategory);
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
            var category = new Categories(NameTypes.ClientCategory);
            category.ShowDialog();
            var dialogueResult = category.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                LoadCategories();
                SelectedCategory = Categories.FirstOrDefault(c => c.DisplayName == category.TxtCategoryName.Text);
                if (SelectedCategory != null) SelectedClient.CategoryId = SelectedCategory.Id;
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
