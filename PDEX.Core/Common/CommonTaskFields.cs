using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEX.Core.Models
{
    public class CommonTaskFields : CommonFieldsA
    { 
        public string Description
        {
            get { return GetValue(() => Description); }
            set { SetValue(() => Description, value); }
        }
        
        public DateTime? StartTime
        {
            get { return GetValue(() => StartTime); }
            set { SetValue(() => StartTime, value); }
        }

        public DateTime? DueDate
        {
            get { return GetValue(() => DueDate); }
            set { SetValue(() => DueDate, value); }
        }

        public DateTime? EndTime
        {
            get { return GetValue(() => EndTime); }
            set { SetValue(() => EndTime, value); }
        }

        [ForeignKey("AssignedTo")]
        public int? AssignedToId { get; set; }
        public StaffDTO AssignedTo
        {
            get { return GetValue(() => AssignedTo); }
            set { SetValue(() => AssignedTo, value); }
        }

        public decimal? PaymentAmount
        {
            get { return GetValue(() => PaymentAmount); }
            set { SetValue(() => PaymentAmount, value); }
        }
        
        [NotMapped]
        public string DescriptionShort
        {
            get { return Description != null && Description.Length > 18 ? Description.Substring(0, 15) + "..." : Description; }
            set { SetValue(() => DescriptionShort, value); }
        }

        [NotMapped]
        public string PaymentStatus
        {
            get { return GetValue(() => PaymentStatus); }
            set { SetValue(() => PaymentStatus, value); }
        }
    }
}