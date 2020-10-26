using System.ComponentModel;

namespace PDEX.Core.Enumerations
{
    public enum TaskProcessTypes
    {
        [Description("Ordinary")]
        Ordinary,
        [Description("Tender")]
        Tender,
        [Description("Delivery")]
        Delivery,
    }
}