using System.ComponentModel;

namespace PDEX.Core.Enumerations
{
    public enum PaymentStatus
    {
        [Description("Cash Not Deposited")]
        NotDeposited = 0,
        [Description("Not Cleared")]
        NotCleared = 1,
        [Description("Cleared")]
        Cleared = 2,
        [Description("No Payment")]//For Draft Status
        NoPayment = 3,
        [Description("Refunded")]
        Refund = 4,
    }
}