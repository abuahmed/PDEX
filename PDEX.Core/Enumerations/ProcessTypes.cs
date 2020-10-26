using System.ComponentModel;

namespace PDEX.Core.Enumerations
{
    public enum ProcessTypes
    {
        [Description("TaskProcess")]
        TaskProcess,
        //[Description("Acceptance")]
        //Acceptance,
        [Description("Delivery")]
        Delivery,
        [Description("Delivery Line")]
        DeliveryLine,
    }
}