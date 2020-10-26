using System.ComponentModel;

namespace PDEX.Core.Enumerations
{
    public enum PaymentTypes
    {
        [Description("Delivery Line")]
        DeliveryLine = 0,
        [Description("Task Process")]
        TaskProcess = 1,
        [Description("Cash Loan")]
        CashIn = 2,
        [Description("Expense")]
        CashOut = 3
    }
}