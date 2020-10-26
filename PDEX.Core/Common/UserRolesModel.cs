using PDEX.Core.Enumerations;
using PDEX.Core.Models;

namespace PDEX.Core.Common
{
    public class UserRolesModel : EntityBase
    {
        public UserRolesModel()
        {
            Admin = CommonUtility.UserHasRole(RoleTypes.Admin) ? "Visible" : "Collapsed";
            Settings = CommonUtility.UserHasRole(RoleTypes.Settings) ? "Visible" : "Collapsed";
            AdvancedSettings = CommonUtility.UserHasRole(RoleTypes.AdvancedSettings) ? "Visible" : "Collapsed";
            
            UsersMgmt = CommonUtility.UserHasRole(RoleTypes.UsersMgmt) ? "Visible" : "Collapsed";
            UsersPrivilegeMgmt = CommonUtility.UserHasRole(RoleTypes.UsersPrivilegeMgmt) ? "Visible" : "Collapsed";
            BackupRestore = CommonUtility.UserHasRole(RoleTypes.BackupRestore) ? "Visible" : "Collapsed";

            AddDelivery = CommonUtility.UserHasRole(RoleTypes.AddDelivery) ? "Visible" : "Collapsed";
            EditDelivery = CommonUtility.UserHasRole(RoleTypes.EditDelivery) ? "Visible" : "Collapsed";
            DeleteDelivery = CommonUtility.UserHasRole(RoleTypes.DeleteDelivery) ? "Visible" : "Collapsed";
            AcceptDelivery = CommonUtility.UserHasRole(RoleTypes.AcceptDelivery) ? "Visible" : "Collapsed";

            AddLine = CommonUtility.UserHasRole(RoleTypes.AddLine) ? "Visible" : "Collapsed";
            EditLine = CommonUtility.UserHasRole(RoleTypes.EditLine) ? "Visible" : "Collapsed";
            DeleteLine = CommonUtility.UserHasRole(RoleTypes.DeleteLine) ? "Visible" : "Collapsed";

            AddMessage = CommonUtility.UserHasRole(RoleTypes.AddMessage) ? "Visible" : "Collapsed";
            EditMessage = CommonUtility.UserHasRole(RoleTypes.EditMessage) ? "Visible" : "Collapsed";
            DeleteMessage = CommonUtility.UserHasRole(RoleTypes.DeleteMessage) ? "Visible" : "Collapsed";
        }

        #region Public Properties
        public string Admin
        {
            get { return GetValue(() => Admin); }
            set { SetValue(() => Admin, value); }
        }
        public string Settings
        {
            get { return GetValue(() => Settings); }
            set { SetValue(() => Settings, value); }
        }
        public string AdvancedSettings
        {
            get { return GetValue(() => AdvancedSettings); }
            set { SetValue(() => AdvancedSettings, value); }
        }
        public string UsersMgmt
        {
            get { return GetValue(() => UsersMgmt); }
            set { SetValue(() => UsersMgmt, value); }
        }
        public string UsersPrivilegeMgmt
        {
            get { return GetValue(() => UsersPrivilegeMgmt); }
            set { SetValue(() => UsersPrivilegeMgmt, value); }
        }
        public string BackupRestore
        {
            get { return GetValue(() => BackupRestore); }
            set { SetValue(() => BackupRestore, value); }
        }

        public string AddDelivery
        {
            get { return GetValue(() => AddDelivery); }
            set { SetValue(() => AddDelivery, value); }
        }
        public string EditDelivery
        {
            get { return GetValue(() => EditDelivery); }
            set { SetValue(() => EditDelivery, value); }
        }
        public string DeleteDelivery
        {
            get { return GetValue(() => DeleteDelivery); }
            set { SetValue(() => DeleteDelivery, value); }
        }

        public string AcceptDelivery
        {
            get { return GetValue(() => AcceptDelivery); }
            set { SetValue(() => AcceptDelivery, value); }
        }

        public string AddLine
        {
            get { return GetValue(() => AddLine); }
            set { SetValue(() => AddLine, value); }
        }
        public string EditLine
        {
            get { return GetValue(() => EditLine); }
            set { SetValue(() => EditLine, value); }
        }
        public string DeleteLine
        {
            get { return GetValue(() => DeleteLine); }
            set { SetValue(() => DeleteLine, value); }
        }

        public string AddMessage
        {
            get { return GetValue(() => AddMessage); }
            set { SetValue(() => AddMessage, value); }
        }
        public string EditMessage
        {
            get { return GetValue(() => EditMessage); }
            set { SetValue(() => EditMessage, value); }
        }
        public string DeleteMessage
        {
            get { return GetValue(() => DeleteMessage); }
            set { SetValue(() => DeleteMessage, value); }
        }
        #endregion
    }
}