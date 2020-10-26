using PDEX.Core;
using PDEX.Core.Enumerations;

namespace PDEX.WPF.ViewModel
{
    public class ViewModelLocator
    {
        private static Bootstrapper _bootStrapper;

        public ViewModelLocator()
        {
            //Add Code to choose the server/database the user wants to connect to, the line below depends on it
            //Singleton.Edition = PDEXEdition.ServerEdition;
            if (_bootStrapper == null)
                _bootStrapper = new Bootstrapper();
        }

        public SplashScreenViewModel Splash
        {
            get
            {
                return _bootStrapper.Container.Resolve<SplashScreenViewModel>();
            }
        }
        public LoginViewModel Login
        {
            get
            {
                return _bootStrapper.Container.Resolve<LoginViewModel>();
            }
        }
        public ChangePasswordViewModel ChangePassword
        {
            get
            {
                return _bootStrapper.Container.Resolve<ChangePasswordViewModel>();
            }
        }

        public MainViewModel Main
        {
            get
            {
                return _bootStrapper.Container.Resolve<MainViewModel>();
            }
        }
        public DeliveryViewModel Delivery
        {
            get
            {
                return _bootStrapper.Container.Resolve<DeliveryViewModel>();
            }
        }
        public TaskProcessViewModel Process
        {
            get
            {
                return _bootStrapper.Container.Resolve<TaskProcessViewModel>();
            }
        }
        public TaskProcessEntryViewModel ProcessEntry
        {
            get
            {
                return _bootStrapper.Container.Resolve<TaskProcessEntryViewModel>();
            }
        }
        public FollowUpViewModel FollowUp
        {
            get
            {
                return _bootStrapper.Container.Resolve<FollowUpViewModel>();
            }
        }
        public CompanyViewModel Company
        {
            get
            {
                return _bootStrapper.Container.Resolve<CompanyViewModel>();
            }
        }
        public ClientViewModel Client
        {
            get
            {
                return _bootStrapper.Container.Resolve<ClientViewModel>();
            }
        }
        public DocumentViewModel Document
        {
            get
            {
                return _bootStrapper.Container.Resolve<DocumentViewModel>();
            }
        }
        public WarehouseViewModel Warehouse
        {
            get
            {
                return _bootStrapper.Container.Resolve<WarehouseViewModel>();
            }
        }
        public StaffViewModel Staff
        {
            get
            {
                return _bootStrapper.Container.Resolve<StaffViewModel>();
            }
        }
        public AddressViewModel AddressVm
        {
            get
            {
                return _bootStrapper.Container.Resolve<AddressViewModel>();
            }
        }
        public OrderByClientViewModel OrderByClientVm
        {
            get
            {
                return _bootStrapper.Container.Resolve<OrderByClientViewModel>();
            }
        }
        public StorageBinViewModel StorageVm
        {
            get
            {
                return _bootStrapper.Container.Resolve<StorageBinViewModel>();
            }
        }
        public DeliveryLineViewModel DeliveryLineVm
        {
            get
            {
                return _bootStrapper.Container.Resolve<DeliveryLineViewModel>();
            }
        }
        public MessageViewModel MessageVm
        {
            get
            {
                return _bootStrapper.Container.Resolve<MessageViewModel>();
            }
        }
        public AcceptanceEntryViewModel AcceptanceVm
        {
            get
            {
                return _bootStrapper.Container.Resolve<AcceptanceEntryViewModel>();
            }
        }
        public QuickProcessViewModel QuickProcessVm
        {
            get
            {
                return _bootStrapper.Container.Resolve<QuickProcessViewModel>();
            }
        }
        public DeliveryEntryViewModel DeliveryVm
        {
            get
            {
                return _bootStrapper.Container.Resolve<DeliveryEntryViewModel>();
            }
        }
        public VehicleViewModel Vehicle
        {
            get
            {
                return _bootStrapper.Container.Resolve<VehicleViewModel>();
            }
        }
        public CategoryViewModel Categories
        {
            get
            {
                return _bootStrapper.Container.Resolve<CategoryViewModel>();
            }
        }

        public ExpenseViewModel ExpenseLoan
        {
            get
            {
                return _bootStrapper.Container.Resolve<ExpenseViewModel>();
            }
        }
        public ExpenseEntryViewModel ExpenseLoanEntry
        {
            get
            {
                return _bootStrapper.Container.Resolve<ExpenseEntryViewModel>();
            }
        }

        public TenderViewModel Tender
        {
            get
            {
                return _bootStrapper.Container.Resolve<TenderViewModel>();
            }
        }
        public TenderEntryViewModel TenderEntry
        {
            get
            {
                return _bootStrapper.Container.Resolve<TenderEntryViewModel>();
            }
        }

        public UserViewModel User
        {
            get
            {
                return _bootStrapper.Container.Resolve<UserViewModel>();
            }
        }
        public BackupRestoreViewModel BackupRestore
        {
            get
            {
                return _bootStrapper.Container.Resolve<BackupRestoreViewModel>();
            }
        }

        public ReportViewerViewModel ReportViewerCommon
        {
            get
            {
                return _bootStrapper.Container.Resolve<ReportViewerViewModel>();
            }
        }
        public static void Cleanup()
        {

        }
    }
}