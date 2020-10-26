using System;
//using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PDEX.Service;
using PDEX.Service.Interfaces;

namespace PDEX.WPF.ViewModel
{
    public class TenderEntryViewModel : ViewModelBase
    {
        #region Fields
        private static ITenderService _tenderService;
        private TenderDTO _selectedTender;
        private string _headerText;
        private ICommand _addNewTenderCommand, _saveTenderCommand, _closeTenderLoanViewCommand;
        #endregion

        #region Constructor
        public TenderEntryViewModel()
        {
            CleanUp();
            _tenderService = new TenderService();
            HeaderText = "Add Tender";
            AddNewTender();
            Messenger.Default.Register<TenderDTO>(this, (message) =>
            {
                SelectedTender = _tenderService.Find(message.Id.ToString(CultureInfo.InvariantCulture));
            });

        }
        public static void CleanUp()
        {
            if (_tenderService != null)
                _tenderService.Dispose();
        }
        #endregion

        #region Public Properties
   

        public TenderDTO SelectedTender
        {
            get { return _selectedTender; }
            set
            {
                _selectedTender = value;
                RaisePropertyChanged<TenderDTO>(() => SelectedTender);
                if (SelectedTender != null && SelectedTender.Id != 0)
                {
                    HeaderText = "Edit Tender";
                }
            }
        }

        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                _headerText = value;
                RaisePropertyChanged<string>(() => HeaderText);
            }
        }
        
        #endregion

        #region Commands
        public ICommand AddNewTenderCommand
        {
            get
            {
                return _addNewTenderCommand ?? (_addNewTenderCommand = new RelayCommand(AddNewTender));
            }
        }
        private void AddNewTender()
        {
            SelectedTender = new TenderDTO
            {
                //Type = TenderType,
                //TenderDate = DateTime.Now,
                //Method = TenderMethods.Cash,
                //Status = TenderStatus.NotCleared
            };
        }

        public ICommand SaveTenderCommand
        {
            get
            {
                return _saveTenderCommand ?? (_saveTenderCommand = new RelayCommand<Object>(SaveTender, CanSave));
            }
        }
        private void SaveTender(object obj)
        {
            _tenderService.InsertOrUpdate(SelectedTender);
            CloseWindow(obj);
        }

        public ICommand CloseTenderLoanViewCommand
        {
            get
            {
                return _closeTenderLoanViewCommand ?? (_closeTenderLoanViewCommand = new RelayCommand<Object>(CloseWindow));
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
        #endregion
        
        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object obj)
        {
            return Errors == 0;
        }
        #endregion
    }
}
