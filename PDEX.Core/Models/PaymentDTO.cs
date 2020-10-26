using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PDEX.Core.Enumerations;
using PDEX.Core.Extensions;

namespace PDEX.Core.Models
{
    public class PaymentDTO : CommonFieldsA
    {
        [ForeignKey("TaskProcess")]
        public int? TaskProcessId { get; set; }
        public TaskProcessDTO TaskProcess
        {
            get { return GetValue(() => TaskProcess); }
            set { SetValue(() => TaskProcess, value); }
        }

        [ForeignKey("DeliveryLine")]
        public int? DeliveryLineId { get; set; }
        public DeliveryLineDTO DeliveryLine
        {
            get { return GetValue(() => DeliveryLine); }
            set { SetValue(() => DeliveryLine, value); }
        }

        [Required]
        public PaymentTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }

        public PaymentMethods Method
        {
            get { return GetValue(() => Method); }
            set { SetValue(() => Method, value); }
        }

        public PaymentStatus Status
        {
            get { return GetValue(() => Status); }
            set { SetValue(() => Status, value); }
        }

        [Required]
        public DateTime PaymentDate
        {
            get { return GetValue(() => PaymentDate); }
            set { SetValue(() => PaymentDate, value); }
        }

        [Required]
        public string Reason
        {
            get { return GetValue(() => Reason); }
            set { SetValue(() => Reason, value); }
        }

        [Required]
        public string PersonName
        {
            get { return GetValue(() => PersonName); }
            set { SetValue(() => PersonName, value); }
        }

        public string PaymentRemark
        {
            get { return GetValue(() => PaymentRemark); }
            set { SetValue(() => PaymentRemark, value); }
        }

        [Required]
        public decimal Amount
        {
            get { return GetValue(() => Amount); }
            set { SetValue(() => Amount, value); }
        }
        
        public DateTime? DueDate
        {
            get { return GetValue(() => DueDate); }
            set { SetValue(() => DueDate, value); }
        }
        
        [NotMapped]
        [DisplayName("Payment Date")]
        public string PaymentDateString
        {
            get
            {
                return PaymentDate.ToString("dd-MM-yyyy") + "(" + ReportUtility.GetEthCalendarFormated(PaymentDate, "/") + ")";
            }
            set { SetValue(() => PaymentDateString, value); }
        }

        [NotMapped]
        [DisplayName("Amount")]
        public string AmountString
        {
            get { return Amount.ToString("C"); }
            set { SetValue(() => AmountString, value); }
        }

        [NotMapped]
        [DisplayName("Payment Type")]
        public string PaymentTypeString
        {
            get
            {
                return EnumUtil.GetEnumDesc(Type);
            }
            set { SetValue(() => PaymentTypeString, value); }
        }

        [NotMapped]
        [DisplayName("Status")]
        public string StatusString
        {
            get
            {
                return EnumUtil.GetEnumDesc(Status);
            }
            set { SetValue(() => StatusString, value); }
        }

        [NotMapped]
        [DisplayName("Payment Method")]
        public string PaymentMethodString
        {
            get
            {
                return EnumUtil.GetEnumDesc(Method);
            }
            set { SetValue(() => StatusString, value); }
        }
    }
}