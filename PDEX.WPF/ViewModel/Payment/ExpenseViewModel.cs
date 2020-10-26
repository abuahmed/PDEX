using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using PDEX.Core;
using PDEX.Core.Common;
using PDEX.Core.Enumerations;
using PDEX.Core.Models;
using PDEX.Service;
using PDEX.Service.Interfaces;
using PDEX.WPF.Properties;
using PDEX.WPF.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MessageBox = System.Windows.MessageBox;

namespace PDEX.WPF.ViewModel
{
    public class ExpenseViewModel : ViewModelBase
    {
        #region Fields
        private ICommand _refreshWindowCommand;
        private static IPaymentService _paymentService;
        #endregion

        #region Constructor
        public ExpenseViewModel()
        {
            FillPeriodCombo();
            SelectedPeriod = FilterPeriods.FirstOrDefault();

            AddNewPaymentCommandVisibility = true;
            Load();
        }

        public void Load()
        {
            CleanUp();
            _paymentService = new PaymentService();
            GetLivePayments();
        }

        public static void CleanUp()
        {
            if (_paymentService != null)
                _paymentService.Dispose();
        }
        public ICommand RefreshWindowCommand
        {
            get
            {
                return _refreshWindowCommand ?? (_refreshWindowCommand = new RelayCommand(Load));
            }
        }
        #endregion

        #region Expenses

        #region Fields
        private int _totalNumberOfPayments;
        private string _totalValueOfPayments;
        private bool _addNewPaymentCommandVisibility, _savePaymentCommandVisibility;
        private PaymentDTO _selectedPayment;
        private IEnumerable<PaymentDTO> _paymentList;
        private ObservableCollection<PaymentDTO> _payments;
        private ICommand _addNewPaymentCommand, _deletePaymentCommand;
        #endregion

        #region Public Properties
        public int TotalNumberOfPayments
        {
            get { return _totalNumberOfPayments; }
            set
            {
                _totalNumberOfPayments = value;
                RaisePropertyChanged<int>(() => TotalNumberOfPayments);
            }
        }
        public string TotalValueOfPayments
        {
            get { return _totalValueOfPayments; }
            set
            {
                _totalValueOfPayments = value;
                RaisePropertyChanged<string>(() => TotalValueOfPayments);
            }
        }
        public bool AddNewPaymentCommandVisibility
        {
            get { return _addNewPaymentCommandVisibility; }
            set
            {
                _addNewPaymentCommandVisibility = value;
                RaisePropertyChanged<bool>(() => AddNewPaymentCommandVisibility);
            }
        }
        public bool SavePaymentCommandVisibility
        {
            get { return _savePaymentCommandVisibility; }
            set
            {
                _savePaymentCommandVisibility = value;
                RaisePropertyChanged<bool>(() => SavePaymentCommandVisibility);
            }
        }
        public PaymentDTO SelectedPayment
        {
            get { return _selectedPayment; }
            set
            {
                _selectedPayment = value;
                RaisePropertyChanged<PaymentDTO>(() => SelectedPayment);
                if (SelectedPayment != null)
                {
                    SavePaymentCommandVisibility = true;
                }
            }
        }
        public ObservableCollection<PaymentDTO> Payments
        {
            get { return _payments; }
            set
            {
                _payments = value;
                RaisePropertyChanged<ObservableCollection<PaymentDTO>>(() => Payments);

                var cashOut = Payments.Where(p => p.Type == PaymentTypes.CashOut).Sum(p => p.Amount);
                var cashIn = Payments.Where(p => p.Type == PaymentTypes.CashIn).Sum(p => p.Amount);

                var diff = cashIn - cashOut;
                TotalValueOfPayments = diff.ToString("N");
            }
        }
        public IEnumerable<PaymentDTO> PaymentList
        {
            get { return _paymentList; }
            set
            {
                _paymentList = value;
                RaisePropertyChanged<IEnumerable<PaymentDTO>>(() => PaymentList);
            }
        }
        #endregion

        #region Commands
        public ICommand AddNewPaymentCommand
        {
            get
            {
                return _addNewPaymentCommand ?? (_addNewPaymentCommand = new RelayCommand<Object>(ExcuteAddNewPaymentCommand));
            }
        }
        private void ExcuteAddNewPaymentCommand(object button)
        {
            try
            {
                var btn = button as System.Windows.Controls.Button;
                if (btn != null)
                {
                    string buttonTag = btn.Tag.ToString();
                    switch (buttonTag)
                    {
                        case "Expense":
                            var expenseEntryWindow = new ExpenseEntry(PaymentTypes.CashOut);
                            expenseEntryWindow.ShowDialog();
                            break;
                  
                        case "Edit":
                            var editWindow = new ExpenseEntry(SelectedPayment);
                            editWindow.ShowDialog();
                            break;
                    }
                }
                Load();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't Add Payment"
                                  + Environment.NewLine + exception.Message, "Can't Add Payment", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand DeletePaymentCommand
        {
            get
            {
                return _deletePaymentCommand ?? (_deletePaymentCommand = new RelayCommand(ExcuteDeletePaymentCommand, CanSave));
            }
        }
        private void ExcuteDeletePaymentCommand()
        {
            try
            {
                SelectedPayment.Enabled = false;
                var stat = _paymentService.InsertOrUpdate(SelectedPayment);

                if (string.IsNullOrEmpty(stat))
                    Load();
                else
                    MessageBox.Show("Can't Delete Payment"
                                 + Environment.NewLine + stat, "Can't Delete Payment", MessageBoxButton.OK,
                     MessageBoxImage.Error);


            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't Delete Payment"
                                  + Environment.NewLine + exception.Message, "Can't Delete Payment", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }
        #endregion

        public void GetLivePayments()
        {
            PaymentList = new List<PaymentDTO>();

            var criteria = new SearchCriteria<PaymentDTO>
            {
                CurrentUserId = Singleton.User.UserId,
                BeginingDate = FilterStartDate,
                EndingDate = FilterEndDate
            };
            criteria.FiList.Add(p => p.Type == PaymentTypes.CashIn || p.Type == PaymentTypes.CashOut);

            if (!string.IsNullOrEmpty(FilterByPerson))
                criteria.FiList.Add(pi => pi.PersonName.Contains(FilterByPerson));

            if (!string.IsNullOrEmpty(FilterByReason))
                criteria.FiList.Add(pi => pi.Reason.Contains(FilterByReason));

            //if (SelectedPaymentType != null)
            //    criteria.PaymentType = SelectedPaymentType.Value;

            PaymentList = _paymentService.GetAll(criteria);

            Payments = new ObservableCollection<PaymentDTO>(PaymentList);

            TotalNumberOfPayments = Payments.Count;

            if (Payments.Count > 0)
                SelectedPayment = Payments.FirstOrDefault();
        }

        #endregion

        
        #region Filter Header

        #region Fields
        private string _filterPeriod;
        private string _filterByPerson, _filterByReason;
        private IList<ListDataItem> _filterPeriods;
        private ListDataItem _selectedPeriod;
        private DateTime _filterStartDate, _filterEndDate;
        #endregion

        #region Public Properties
        public string FilterPeriod
        {
            get { return _filterPeriod; }
            set
            {
                _filterPeriod = value;
                RaisePropertyChanged<string>(() => FilterPeriod);
            }
        }
        public DateTime FilterStartDate
        {
            get { return _filterStartDate; }
            set
            {
                _filterStartDate = value;
                RaisePropertyChanged<DateTime>(() => FilterStartDate);
            }
        }
        public DateTime FilterEndDate
        {
            get { return _filterEndDate; }
            set
            {
                _filterEndDate = value;
                RaisePropertyChanged<DateTime>(() => FilterEndDate);
            }
        }

        public IList<ListDataItem> FilterPeriods
        {
            get { return _filterPeriods; }
            set
            {
                _filterPeriods = value;
                RaisePropertyChanged<IList<ListDataItem>>(() => FilterPeriods);
            }
        }
        public ListDataItem SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                _selectedPeriod = value;
                RaisePropertyChanged<ListDataItem>(() => SelectedPeriod);
                if (SelectedPeriod == null) return;
                switch (SelectedPeriod.Value)
                {
                    case 0:
                        FilterStartDate = new DateTime(2014, 1, 1);
                        FilterEndDate = new DateTime(2016, 1, 1);
                        break;
                    case 1:
                        FilterStartDate = DateTime.Now;
                        FilterEndDate = DateTime.Now;
                        break;
                    case 2:
                        FilterStartDate = DateTime.Now.AddDays(-1);
                        FilterEndDate = DateTime.Now.AddDays(-1);
                        break;
                    case 3:
                        FilterStartDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                        FilterEndDate = DateTime.Now.AddDays(7 - (int)DateTime.Now.DayOfWeek - 1);
                        break;
                    case 4:
                        FilterStartDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 7);
                        FilterEndDate = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 1);
                        break;
                }
            }
        }
        public string FilterByPerson
        {
            get { return _filterByPerson; }
            set
            {
                _filterByPerson = value;
                RaisePropertyChanged<string>(() => FilterByPerson);
            }
        }
        public string FilterByReason
        {
            get { return _filterByReason; }
            set
            {
                _filterByReason = value;
                RaisePropertyChanged<string>(() => FilterByReason);
            }
        }
        #endregion
        
        private void FillPeriodCombo()
        {
            FilterPeriods = new List<ListDataItem>
            {
                new ListDataItem {Display = "All/Custom", Value = 0},
                new ListDataItem {Display = "Today", Value = 1},
                new ListDataItem {Display = "Yesterday", Value = 2},
                new ListDataItem {Display = "This Week", Value = 3},
                new ListDataItem {Display = "Last Week", Value = 4}
            };
        }

        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave()
        {
            return Errors == 0;
        }
        #endregion
        
        #region Export To Excel
        private ICommand _exportToExcelCommand;
        public ICommand ExportToExcelCommand
        {
            get { return _exportToExcelCommand ?? (_exportToExcelCommand = new RelayCommand(ExecuteExportToExcelCommand)); }
        }

        private void ExecuteExportToExcelCommand()
        {
            string[] columnsHeader = { "S.No.", "On Date", "Reason", "To", "Amount" };


            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            var xlApp = new Microsoft.Office.Interop.Excel.Application();

            try
            {
                Microsoft.Office.Interop.Excel.Workbook excelBook = xlApp.Workbooks.Add();
                var excelWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.Worksheets[1];
                xlApp.Visible = true;

                var rowsTotal = Payments.Count;
                var colsTotal = columnsHeader.Count();

                var with1 = excelWorksheet;
                with1.Cells.Select();
                with1.Cells.Delete();

                int iC;
                for (iC = 0; iC <= colsTotal - 1; iC++)
                {
                    with1.Cells[1, iC + 1].Value = columnsHeader[iC];
                }

                int I;
                for (I = 0; I <= rowsTotal - 1; I++)
                {
                    with1.Cells[I + 2, 0 + 1].value = (I+1).ToString(CultureInfo.InvariantCulture);
                    with1.Cells[I + 2, 1 + 1].value = Payments[I].PaymentDateString;
                    with1.Cells[I + 2, 2 + 1].value = Payments[I].Reason;
                    with1.Cells[I + 2, 3 + 1].value = Payments[I].PersonName;
                    with1.Cells[I + 2, 4 + 1].value = Payments[I].Amount;
                }

                with1.Rows["1:1"].Font.FontStyle = "Bold";
                with1.Rows["1:1"].Font.Size = 12;

                with1.Cells.Columns.AutoFit();
                with1.Cells.Select();
                with1.Cells.EntireColumn.AutoFit();
                with1.Cells[1, 1].Select();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, Resources.Exporting_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //RELEASE ALLOACTED RESOURCES
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                xlApp = null;
            }
        }
        #endregion
    }
}
