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
    public class ExpenseEntryViewModel : ViewModelBase
    {
        #region Fields
        private static IPaymentService _paymentService;
        private PaymentTypes _paymentType;
        private PaymentDTO _selectedPayment;
        private string _headerText;
        private ICommand _addNewPaymentCommand, _savePaymentCommand, _closeExpenseLoanViewCommand;
        #endregion

        #region Constructor
        public ExpenseEntryViewModel()
        {
            CleanUp();
            _paymentService = new PaymentService();

            Messenger.Default.Register<PaymentTypes>(this, (message) =>
            {
                PaymentType = message;
            });
            Messenger.Default.Register<PaymentDTO>(this, (message) =>
            {
                SelectedPayment = _paymentService.Find(message.Id.ToString(CultureInfo.InvariantCulture));
            });

        }
        public static void CleanUp()
        {
            if (_paymentService != null)
                _paymentService.Dispose();
        }
        #endregion

        #region Public Properties
        public PaymentTypes PaymentType
        {
            get { return _paymentType; }
            set
            {
                _paymentType = value;
                RaisePropertyChanged<PaymentTypes>(() => PaymentType);

                HeaderText = "Add Expense";
                AddNewPayment();
            }
        }

        public PaymentDTO SelectedPayment
        {
            get { return _selectedPayment; }
            set
            {
                _selectedPayment = value;
                RaisePropertyChanged<PaymentDTO>(() => SelectedPayment);
                if (SelectedPayment != null && SelectedPayment.Id != 0)
                {
                    HeaderText = "Edit Expense";
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
        public ICommand AddNewPaymentCommand
        {
            get
            {
                return _addNewPaymentCommand ?? (_addNewPaymentCommand = new RelayCommand(AddNewPayment));
            }
        }
        private void AddNewPayment()
        {
            SelectedPayment = new PaymentDTO
            {
                Type = PaymentType,
                PaymentDate = DateTime.Now,
                Method = PaymentMethods.Cash,
                Status = PaymentStatus.NotCleared
            };
        }

        public ICommand SavePaymentCommand
        {
            get
            {
                return _savePaymentCommand ?? (_savePaymentCommand = new RelayCommand<Object>(SavePayment, CanSave));
            }
        }
        private void SavePayment(object obj)
        {
            _paymentService.InsertOrUpdate(SelectedPayment);
            CloseWindow(obj);
        }

        public ICommand CloseExpenseLoanViewCommand
        {
            get
            {
                return _closeExpenseLoanViewCommand ?? (_closeExpenseLoanViewCommand = new RelayCommand<Object>(CloseWindow));
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
