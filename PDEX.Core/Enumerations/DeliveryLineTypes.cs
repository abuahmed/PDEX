using System.ComponentModel;

namespace PDEX.Core.Enumerations
{
    public enum DeliveryLineRangeTypes
    {
        All,
        [Description("Inside City")]
        InsideCity,
        [Description("City to City")]
        OutSideCity
    }
}