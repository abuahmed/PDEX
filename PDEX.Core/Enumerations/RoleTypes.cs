using System.ComponentModel;

namespace PDEX.Core.Enumerations
{
    public enum RoleTypes
    {
        [Description("System Administration")]
        Admin,

        [Description("Options")]
        Settings,
        [Description("Advanced Options")]
        AdvancedSettings,

        [Description("Users Mgmt")]
        UsersMgmt,
        [Description("Users Privilege Mgmt")]
        UsersPrivilegeMgmt,

        [Description("Backup and Restore Mgmt")]
        BackupRestore,

        [Description("Staffs Mgmt")]
        Staffs,
        [Description("Advanced Staffs Mgmt")]
        StaffsAdvanced,
        [Description("Vehicles Mgmt")]
        Vehicles,
        [Description("Advanced Vehicles Mgmt")]
        VehiclesAdvanced,
        [Description("Clients Mgmt")]
        Clients,
        [Description("Advanced Clients Mgmt")]
        ClientsAdvanced,

        [Description("Add Delivery")]
        AddDelivery,
        [Description("Edit Delivery")]
        EditDelivery,
        [Description("Delete Delivery")]
        DeleteDelivery,
        [Description("Accept Delivery")]
        AcceptDelivery,

        [Description("Add Line")]
        AddLine,
        [Description("Edit Line")]
        EditLine,
        [Description("Delete Line")]
        DeleteLine,

        [Description("Add Message")]
        AddMessage,
        [Description("Edit Message")]
        EditMessage,
        [Description("Delete Message")]
        DeleteMessage,

    }
}
