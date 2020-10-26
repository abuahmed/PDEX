using System.ComponentModel;

namespace PDEX.Core.Enumerations
{
    public enum DeliveryLineRouteTypes
    {
        //All,
        [Description("To Accept From Sender/Staff")]
        Accepting,
        [Description("To Give To Receiver/Staff")]
        Delivering
    }
}