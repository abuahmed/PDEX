using System.ComponentModel;

namespace PDEX.Core.Enumerations
{
    public enum DeliveryStatusTypes
    {
        [Description("Ordered")]
        Ordered,
        [Description("Acceptance Scheduled(ተቀባይ ተመድቧል)")]
        AcceptanceScheduled,
        [Description("On Acceptance(እየተቀበልን)")]
        OnAcceptance,
        [Description("Accepted(ተቀብለናል)")]
        Accepted,
        [Description("Delivery Scheduled(አድራሸ ተመድቧል)")]
        DeliveryScheduled,
        [Description("On Delivery(እያደረስን)")]
        OnDelivery,
        [Description("Delivered(አድርሰናል)")]
        Delivered
    }
}