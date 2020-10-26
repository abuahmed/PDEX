using System.ComponentModel;

namespace PDEX.Core.Enumerations
{
    public enum StaffTypes
    {
        [Description("All")]
        All,
        [Description("Driver")]
        Driver,
        [Description("On Foot")]
        OnFoot,
        [Description("Multi Task")]
        Multi,
        [Description("Office Work")]
        OfficeStaff,
        //[Description("Team Leader")]
        //TeamLeader,
        //[Description("Manager")]
        //Manager,
        //[Description("CEO")]
        //CEO
    }
}