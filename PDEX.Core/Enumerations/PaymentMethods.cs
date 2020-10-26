using System.ComponentModel;

namespace PDEX.Core.Enumerations
{
    public enum PaymentMethods
    {
        [Description("Cash")]
        Cash = 0,
        [Description("Credit")]
        Credit = 1,
        [Description("Check")]
        Check = 2
    }
}