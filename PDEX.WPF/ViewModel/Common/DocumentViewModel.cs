using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using PDEX.Core;
using PDEX.Core.Common;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;
using PDEX.Service;
using PDEX.Service.Interfaces;
using PDEX.WPF.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PDEX.WPF.ViewModel
{
    public class DocumentViewModel : ViewModelBase
    {
        #region Fields
        private static IDocumentService _documentService;
        private static IClientService _clientService;
        private DocumentDTO _selecteddocument;
        private ObservableCollection<DocumentDTO> _documents;
        private ICommand _addNewAccountCommand, _saveAccountCommand, _deleteAccountCommand;
        #endregion

        #region Constructor
        public DocumentViewModel()
        {
            CleanUp();
            _documentService = new DocumentService();
            _clientService = new ClientService();

            CheckRoles();
            LoadDocumentCategories();

            Messenger.Default.Register<ClientDTO>(this, (message) =>
            {
                SelectedClientParam = message;
            });
        }
        public static void CleanUp()
        {
            if (_documentService != null)
                _documentService.Dispose();
            if (_clientService != null)
                _clientService.Dispose();
        }
        #endregion

        #region Properties
        
        public ObservableCollection<DocumentDTO> Documents
        {
            get { return _documents; }
            set
            {
                _documents = value;
                RaisePropertyChanged<ObservableCollection<DocumentDTO>>(() => Documents);
                ExecuteAddNewAccountCommand();
            }
        }
        public DocumentDTO SelectedDocument
        {
            get { return _selecteddocument; }
            set
            {
                _selecteddocument = value;
                RaisePropertyChanged<DocumentDTO>(() => SelectedDocument);
                if (SelectedDocument != null)
                {
                    SelectedDocumentCategory = DocumentCategorys.FirstOrDefault(b => b.Id == SelectedDocument.CategoryId);
                }
            }
        }
        #endregion

        #region Commands
        public ICommand AddNewDocumentCommand
        {
            get { return _addNewAccountCommand ?? (_addNewAccountCommand = new RelayCommand(ExecuteAddNewAccountCommand)); }
        }
        private void ExecuteAddNewAccountCommand()
        {
            SelectedDocument = new DocumentDTO()
            {
                ClientId = SelectedClient.Id,
                Unit = 1
            };

            if (DocumentCategorys != null)
            {
                SelectedDocumentCategory = DocumentCategorys.FirstOrDefault();
            }

        }

        public ICommand SaveDocumentCommand
        {
            get { return _saveAccountCommand ?? (_saveAccountCommand = new RelayCommand<Object>(ExecuteSaveAccountCommand, CanSave)); }
        }
        private void ExecuteSaveAccountCommand(object obj)
        {
            try
            {
                SelectedDocument.CategoryId = SelectedDocumentCategory.Id;

                var isNewObject = SelectedDocument.Id;
                var stat = _documentService.InsertOrUpdate(SelectedDocument);

                if (string.IsNullOrEmpty(stat))
                {
                    //if (isNewObject == 0)
                    //{
                    //    Documents.Insert(0, SelectedDocument);
                    //}
                    GetDocuments();
                }
                else
                {
                    MessageBox.Show("Got Problem while saving, try again..." + Environment.NewLine + stat, "save error", MessageBoxButton.OK,
                      MessageBoxImage.Error);
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show("Got Problem while saving, try again..." + Environment.NewLine + exception.Message, "save error", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand DeleteDocumentCommand
        {
            get { return _deleteAccountCommand ?? (_deleteAccountCommand = new RelayCommand<Object>(ExecuteDeleteAccountCommand, CanSave)); }
        }
        private void ExecuteDeleteAccountCommand(object obj)
        {
            if (MessageBox.Show("Are you Sure You want to delete?", "Delete Account",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            try
            {
                SelectedDocument.Enabled = false;
                _documentService.Disable(SelectedDocument);
                GetDocuments();
            }
            catch
            {
                MessageBox.Show("Can't delete the account, may be the account is already in use...", "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetDocuments()
        {
            var criteria = new SearchCriteria<DocumentDTO>()
            {
                CurrentUserId = Singleton.User.UserId
            };

            criteria.FiList.Add(d => d.ClientId == SelectedClient.Id);

            var documentsList = _documentService.GetAll(criteria).ToList();
            Documents = new ObservableCollection<DocumentDTO>(documentsList.OrderByDescending(f => f.Id));
            int sno = 1;
            foreach (var documentDTO in Documents)
            {
                documentDTO.SerialNumber = sno;
                sno++;
            }
        }
        #endregion

        #region Clients

        private ClientDTO _selectedClientParam, _selectedClient;
        public ClientDTO SelectedClientParam
        {
            get { return _selectedClientParam; }
            set
            {
                _selectedClientParam = value;
                RaisePropertyChanged<ClientDTO>(() => SelectedClientParam);
                if (SelectedClientParam != null)
                {
                    var criteria = new SearchCriteria<ClientDTO>();
                    criteria.FiList.Add(b => b.IsReceiver == false && b.Id == SelectedClientParam.Id);

                    SelectedClient = _clientService.GetAll(criteria).OrderBy(i => i.Id).FirstOrDefault();
                }
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
                    GetDocuments();
                }
            }
        }


        #endregion

        #region Document Categories
        private CategoryDTO _selectedDocumentCategory;
        private ObservableCollection<CategoryDTO> _banks;
        private ICommand _addNewDocumentCategoryCommand;

        public CategoryDTO SelectedDocumentCategory
        {
            get { return _selectedDocumentCategory; }
            set
            {
                _selectedDocumentCategory = value;
                RaisePropertyChanged<CategoryDTO>(() => SelectedDocumentCategory);
            }
        }
        public ObservableCollection<CategoryDTO> DocumentCategorys
        {
            get { return _banks; }
            set
            {
                _banks = value;
                RaisePropertyChanged<ObservableCollection<CategoryDTO>>(() => DocumentCategorys);
            }
        }
        public ICommand AddNewDocumentCategoryCommand
        {
            get
            {
                return _addNewDocumentCategoryCommand ?? (_addNewDocumentCategoryCommand = new RelayCommand(ExcuteAddNewCategoryCommand));
            }
        }
        private void ExcuteAddNewCategoryCommand()
        {
            var category = new Categories(NameTypes.Document);
            category.ShowDialog();
            var dialogueResult = category.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                LoadDocumentCategories();
                SelectedDocumentCategory = DocumentCategorys.FirstOrDefault(b => b.DisplayName.ToLower().Contains(category.TxtCategoryName.Text.ToLower()));
            }

        }

        public void LoadDocumentCategories()
        {
            var criteria = new SearchCriteria<CategoryDTO>();
            criteria.FiList.Add(c => c.NameType == NameTypes.Document);

            IEnumerable<CategoryDTO> banksList = new CategoryService(true).GetAll(criteria)
                .OrderBy(i => i.DisplayName)
                .ToList();
            DocumentCategorys = new ObservableCollection<CategoryDTO>(banksList);
        }
        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object obj)
        {
            return Errors == 0;

        }
        
        #endregion

        #region Previlege Visibility

        private bool _storeNameEnability;
        public bool StoreNameEnability
        {
            get { return _storeNameEnability; }
            set
            {
                _storeNameEnability = value;
                RaisePropertyChanged<bool>(() => StoreNameEnability);
            }
        }
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