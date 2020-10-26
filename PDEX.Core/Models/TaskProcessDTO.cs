using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace PDEX.Core.Models
{
    public class TaskProcessDTO : CommonTaskFields
    {
        [ForeignKey("Client")]
        public int? ClientId { get; set; }
        public ClientDTO Client
        {
            get { return GetValue(() => Client); }
            set { SetValue(() => Client, value); }
        }

        [ForeignKey("TenderItem")]
        public int? TenderItemId { get; set; }
        public TenderItemDTO TenderItem
        {
            get { return GetValue(() => TenderItem); }
            set { SetValue(() => TenderItem, value); }
        }

        [ForeignKey("TaskProcessCategory")]
        public int? TaskProcessCategoryId { get; set; }
        public CategoryDTO TaskProcessCategory
        {
            get { return GetValue(() => TaskProcessCategory); }
            set { SetValue(() => TaskProcessCategory, value); }
        }

        public string CompanyName
        {
            get { return GetValue(() => CompanyName); }
            set { SetValue(() => CompanyName, value); }
        }

        [ForeignKey("CompanyAddress")]
        public int? CompanyAddressId { get; set; }
        public AddressDTO CompanyAddress
        {
            get { return GetValue(() => CompanyAddress); }
            set { SetValue(() => CompanyAddress, value); }
        }

        public DateTime? OrderTime
        {
            get { return GetValue(() => OrderTime); }
            set { SetValue(() => OrderTime, value); }
        }
        
        [NotMapped]
        [DisplayName("Process No.")]
        [MaxLength(10, ErrorMessage = "Exceeded 10 letters")]
        public string Number
        {
            get
            {
                var pref = Id.ToString(CultureInfo.InvariantCulture);
                if (Id < 1000)
                {
                    var id = Id + 10000;
                    pref = id.ToString(CultureInfo.InvariantCulture);
                    pref = pref.Substring(1);
                }
                return "P" + pref;
            }
            set
            {
                SetValue(() => Number, value);
            }
        }
       
        [NotMapped]
        public string ProcessDetail
        {
            get
            {
                var clDet = DescriptionShort + " - " + Number;
                //if (Client != null)
                  //  clDet = clDet + " - " + Client.DisplayNameShort;
                return clDet;
            }
            set { SetValue(() => ProcessDetail, value); }
        }

        public ICollection<PaymentDTO> PaymentsandExpenses
        {
            get { return GetValue(() => PaymentsandExpenses); }
            set { SetValue(() => PaymentsandExpenses, value); }
        }

        //public ICollection<PaymentDTO> Expenses
        //{
        //    get { return GetValue(() => Expenses); }
        //    set { SetValue(() => Expenses, value); }
        //}

        //public ICollection<SubTaskProcessDTO> SubTasks
        //{
        //    get { return GetValue(() => SubTasks); }
        //    set { SetValue(() => SubTasks, value); }
        //}
    }
}