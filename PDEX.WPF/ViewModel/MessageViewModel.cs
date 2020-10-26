#region MyRegion
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
using PDEX.WPF.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
#endregion

namespace PDEX.WPF.ViewModel
{
    public class MessageViewModel : ViewModelBase
    {
        #region Fields
        private static IDeliveryService _deliveryService;
        private MessageDTO _selectedMessage, _selectedMessageParam;
        private ICommand _addMessageCommand, _closeMessageViewCommand, _addNewCategoryCommand, _addNewUomCommand;
        private ObservableCollection<CategoryDTO> _categories, _unitOfMeasure;
        #endregion

        #region Constructor
        public MessageViewModel()
        {
            CleanUp();
            _deliveryService = new DeliveryService();

            CheckRoles();

            LoadCategories();
            if (Categories != null) SelectedCategory = Categories.FirstOrDefault();
            LoadUoMs();
            if (UnitOfMeasures != null) SelectedUnitOfMeasure = UnitOfMeasures.FirstOrDefault();

            Messenger.Default.Register<MessageDTO>(this, (message) =>
            {
                SelectedMessageParam = message;
            });
        }

        public static void CleanUp()
        {
            if (_deliveryService != null)
                _deliveryService.Dispose();
        }
        #endregion

        #region Properties

        public MessageDTO SelectedMessageParam
        {
            get { return _selectedMessageParam; }
            set
            {
                _selectedMessageParam = value;
                RaisePropertyChanged<MessageDTO>(() => SelectedMessageParam);
                if (SelectedMessageParam == null)
                    return;

                if (SelectedMessageParam.Id != 0)
                {
                    var criteria = new SearchCriteria<MessageDTO>
                    {
                        CurrentUserId = Singleton.User.UserId
                    };
                    criteria.FiList.Add(dh => dh.Id == SelectedMessageParam.Id);

                    int totCount;
                    SelectedMessage = _deliveryService.GetAllMessageChilds(criteria, out totCount).FirstOrDefault();
                }
                else
                {
                    SelectedMessage = new MessageDTO()
                    {
                        Unit = 1,
                        //Number = CommonUtility.GetSecretCode(),
                        DeliveryLineId = SelectedMessageParam.DeliveryLineId
                    };
                }
            }
        }

        public MessageDTO SelectedMessage
        {
            get { return _selectedMessage; }
            set
            {
                _selectedMessage = value;
                RaisePropertyChanged<MessageDTO>(() => SelectedMessage);
                if (SelectedMessage != null && SelectedMessage.Id != 0)
                {
                    SelectedCategory = Categories.FirstOrDefault(c => c.Id == SelectedMessage.CategoryId);
                    SelectedUnitOfMeasure = UnitOfMeasures.FirstOrDefault(c => c.Id == SelectedMessage.UnitOfMeasureId);
                }
            }
        }

        #endregion

        #region Commands
        public ICommand SaveMessageCommand
        {
            get { return _addMessageCommand ?? (_addMessageCommand = new RelayCommand<Object>(ExecuteSaveItemViewCommand, CanSave)); }
        }
        private void ExecuteSaveItemViewCommand(object obj)
        {
            try
            {
                SelectedMessage.CategoryId = SelectedCategory.Id;
                SelectedMessage.UnitOfMeasureId = SelectedUnitOfMeasure.Id;

                var stat = _deliveryService.InsertOrUpdateMessageChild(SelectedMessage);
                if (stat == string.Empty)
                {
                    CloseWindow(obj);
                }
                else
                    MessageBox.Show("Got Problem while saving message, try again..." + Environment.NewLine + stat, "save error", MessageBoxButton.OK,
                       MessageBoxImage.Error);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Problem saving Item..." +
                            Environment.NewLine + exception.Message +
                            Environment.NewLine + exception.InnerException);
            }
        }

        public ICommand CloseMessageViewCommand
        {
            get
            {
                return _closeMessageViewCommand ?? (_closeMessageViewCommand = new RelayCommand<Object>(CloseWindow2));
            }
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
        public void CloseWindow2(object obj)
        {
            if (obj != null)
            {
                var window = obj as Window;
                if (window != null)
                {
                    window.DialogResult = false;
                    window.Close();
                }
            }
        }

        #endregion

        #region Categories
        public void LoadCategories()
        {
            var criteria = new SearchCriteria<CategoryDTO>();
            criteria.FiList.Add(c => c.NameType == NameTypes.Category);
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
        public ObservableCollection<CategoryDTO> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                RaisePropertyChanged<ObservableCollection<CategoryDTO>>(() => Categories);
            }
        }
        public ICommand AddNewCategoryCommand
        {
            get
            {
                return _addNewCategoryCommand ?? (_addNewCategoryCommand = new RelayCommand(ExcuteAddNewCategoryCommand));
            }
        }
        private void ExcuteAddNewCategoryCommand()
        {
            var category = new Categories(NameTypes.Category);
            category.ShowDialog();
            var dialogueResult = category.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                LoadCategories();//should also get the latest updates in each row
                SelectedCategory = Categories.FirstOrDefault(c => c.DisplayName == category.TxtCategoryName.Text);
                if (SelectedCategory != null) SelectedMessage.CategoryId = SelectedCategory.Id;
            }
        }
        #endregion

        #region Unit Of Measures
        public void LoadUoMs()
        {
            var criteria = new SearchCriteria<CategoryDTO>();
            criteria.FiList.Add(c => c.NameType == NameTypes.UnitOfMeasure);
            IEnumerable<CategoryDTO> categoriesList = new CategoryService(true)
                .GetAll(criteria)
                .OrderBy(i => i.Id)
                .ToList();

            UnitOfMeasures = new ObservableCollection<CategoryDTO>(categoriesList);
        }

        private CategoryDTO _selectedUnitOfMeasure;
        public CategoryDTO SelectedUnitOfMeasure
        {
            get { return _selectedUnitOfMeasure; }
            set
            {
                _selectedUnitOfMeasure = value;
                RaisePropertyChanged<CategoryDTO>(() => SelectedUnitOfMeasure);
            }
        }
        public ObservableCollection<CategoryDTO> UnitOfMeasures
        {
            get { return _unitOfMeasure; }
            set
            {
                _unitOfMeasure = value;
                RaisePropertyChanged<ObservableCollection<CategoryDTO>>(() => UnitOfMeasures);
            }
        }
        public ICommand AddNewUomCommand
        {
            get
            {
                return _addNewUomCommand ?? (_addNewUomCommand = new RelayCommand(ExcuteAddNewUomCommand));
            }
        }
        private void ExcuteAddNewUomCommand()
        {
            var category = new Categories(NameTypes.UnitOfMeasure);
            category.ShowDialog();
            var dialogueResult = category.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                LoadUoMs();
                SelectedUnitOfMeasure = UnitOfMeasures.FirstOrDefault(c => c.DisplayName == category.TxtCategoryName.Text);
                if (SelectedUnitOfMeasure != null) SelectedMessage.UnitOfMeasureId = SelectedUnitOfMeasure.Id;
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
