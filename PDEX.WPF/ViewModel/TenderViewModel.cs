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
    public class TenderViewModel : ViewModelBase
    {
        #region Fields
        private ICommand _refreshWindowCommand;
        private static ITenderService _paymentService;
        #endregion

        #region Constructor
        public TenderViewModel()
        {
            FillPeriodCombo();
            SelectedPeriod = FilterPeriods.FirstOrDefault();

            AddNewTenderCommandVisibility = true;
            Load();
        }

        public void Load()
        {
            CleanUp();
            _paymentService = new TenderService();
            GetLiveTenders();
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

        #region Tenders

        #region Fields
        private int _totalNumberOfTenders;
        private string _totalValueOfTenders;
        private bool _addNewTenderCommandVisibility, _saveTenderCommandVisibility;
        private TenderDTO _selectedTender;
        private IEnumerable<TenderDTO> _paymentList;
        private ObservableCollection<TenderDTO> _payments;
        private ICommand _addNewTenderCommand, _deleteTenderCommand;
        #endregion

        #region Public Properties
        public int TotalNumberOfTenders
        {
            get { return _totalNumberOfTenders; }
            set
            {
                _totalNumberOfTenders = value;
                RaisePropertyChanged<int>(() => TotalNumberOfTenders);
            }
        }
        public string TotalValueOfTenders
        {
            get { return _totalValueOfTenders; }
            set
            {
                _totalValueOfTenders = value;
                RaisePropertyChanged<string>(() => TotalValueOfTenders);
            }
        }
        public bool AddNewTenderCommandVisibility
        {
            get { return _addNewTenderCommandVisibility; }
            set
            {
                _addNewTenderCommandVisibility = value;
                RaisePropertyChanged<bool>(() => AddNewTenderCommandVisibility);
            }
        }
        public bool SaveTenderCommandVisibility
        {
            get { return _saveTenderCommandVisibility; }
            set
            {
                _saveTenderCommandVisibility = value;
                RaisePropertyChanged<bool>(() => SaveTenderCommandVisibility);
            }
        }
        public TenderDTO SelectedTender
        {
            get { return _selectedTender; }
            set
            {
                _selectedTender = value;
                RaisePropertyChanged<TenderDTO>(() => SelectedTender);
                if (SelectedTender != null)
                {
                    SaveTenderCommandVisibility = true;
                }
            }
        }
        public ObservableCollection<TenderDTO> Tenders
        {
            get { return _payments; }
            set
            {
                _payments = value;
                RaisePropertyChanged<ObservableCollection<TenderDTO>>(() => Tenders);

                //var cashOut = Tenders.Where(p => p.Type == TenderTypes.CashOut).Sum(p => p.Amount);
                //var cashIn = Tenders.Where(p => p.Type == TenderTypes.CashIn).Sum(p => p.Amount);

                //var diff = cashIn - cashOut;
                //TotalValueOfTenders = diff.ToString("N");
            }
        }
        public IEnumerable<TenderDTO> TenderList
        {
            get { return _paymentList; }
            set
            {
                _paymentList = value;
                RaisePropertyChanged<IEnumerable<TenderDTO>>(() => TenderList);
            }
        }
        #endregion

        #region Commands
        public ICommand AddNewTenderCommand
        {
            get
            {
                return _addNewTenderCommand ?? (_addNewTenderCommand = new RelayCommand<Object>(ExcuteAddNewTenderCommand));
            }
        }
        private void ExcuteAddNewTenderCommand(object button)
        {
            try
            {
                var btn = button as System.Windows.Controls.Button;
                if (btn != null)
                {
                    string buttonTag = btn.Tag.ToString();
                    switch (buttonTag)
                    {
                        case "Tender":
                            var tenderEntryWindow = new TenderEntry();
                            tenderEntryWindow.ShowDialog();
                            break;
                  
                        case "Edit":
                            var editWindow = new TenderEntry(SelectedTender);
                            editWindow.ShowDialog();
                            break;
                    }
                }
                Load();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't Add Tender"
                                  + Environment.NewLine + exception.Message, "Can't Add Tender", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }

        public ICommand DeleteTenderCommand
        {
            get
            {
                return _deleteTenderCommand ?? (_deleteTenderCommand = new RelayCommand(ExcuteDeleteTenderCommand, CanSave));
            }
        }
        private void ExcuteDeleteTenderCommand()
        {
            try
            {
                SelectedTender.Enabled = false;
                var stat = _paymentService.InsertOrUpdate(SelectedTender);

                if (string.IsNullOrEmpty(stat))
                    Load();
                else
                    MessageBox.Show("Can't Delete Tender"
                                 + Environment.NewLine + stat, "Can't Delete Tender", MessageBoxButton.OK,
                     MessageBoxImage.Error);


            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't Delete Tender"
                                  + Environment.NewLine + exception.Message, "Can't Delete Tender", MessageBoxButton.OK,
                      MessageBoxImage.Error);
            }
        }
        #endregion

        public void GetLiveTenders()
        {
            TenderList = new List<TenderDTO>();

            var criteria = new SearchCriteria<TenderDTO>
            {
                CurrentUserId = Singleton.User.UserId,
                BeginingDate = FilterStartDate,
                EndingDate = FilterEndDate
            };
      
            TenderList = _paymentService.GetAll(criteria);

            Tenders = new ObservableCollection<TenderDTO>(TenderList);

            TotalNumberOfTenders = Tenders.Count;

            if (Tenders.Count > 0)
                SelectedTender = Tenders.FirstOrDefault();
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

                var rowsTotal = Tenders.Count;
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
                    with1.Cells[I + 2, 1 + 1].value = "";// Tenders[I].TenderDateString;
                    with1.Cells[I + 2, 2 + 1].value = "";// Tenders[I].Reason;
                    with1.Cells[I + 2, 3 + 1].value = "";// Tenders[I].PersonName;
                    with1.Cells[I + 2, 4 + 1].value = "";// Tenders[I].Amount;
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
