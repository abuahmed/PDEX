using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using PDEX.Core.Enumerations;
using System.Linq;

namespace PDEX.Core.Models
{
    public class DeliveryHeaderDTO : CommonFieldsA
    {
        public DeliveryHeaderDTO()
        {
            DeliveryLines = new List<DeliveryLineDTO>();
            SecretCode = CommonUtility.GetSecretCode();
        }

        public AcceptanceTypes AcceptanceType
        {
            get { return GetValue(() => AcceptanceType); }
            set { SetValue(() => AcceptanceType, value); }
        }

        [NotMapped]
        [DisplayName("Delivery No.")]
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
                return "D" + pref;
            }
            set { SetValue(() => Number, value); }
        }

        [DisplayName("Secret Code")]
        [MaxLength(5, ErrorMessage = "Exceeded 5 letters")]
        public string SecretCode
        {
            get { return GetValue(() => SecretCode); }
            set { SetValue(() => SecretCode, value); }
        }

        [ForeignKey("OrderByClient")]
        public int? OrderByClientId { get; set; }
        public ClientDTO OrderByClient
        {
            get { return GetValue(() => OrderByClient); }
            set { SetValue(() => OrderByClient, value); }
        }

        public DeliveryStatusTypes Status
        {
            get { return GetValue(() => Status); }
            set { SetValue(() => Status, value); }
        }

        public DateTime OrderDate
        {
            get { return GetValue(() => OrderDate); }
            set { SetValue(() => OrderDate, value); }
        }

        public bool DeliverDirectly
        {
            get { return GetValue(() => DeliverDirectly); }
            set { SetValue(() => DeliverDirectly, value); }
        }

        public string MoreComments
        {
            get { return GetValue(() => MoreComments); }
            set { SetValue(() => MoreComments, value); }
        }

        [NotMapped]
        public string DeliveryDetail
        {
            get
            {
                string del = Number;
                if (OrderByClient != null)
                {
                    del = del + " - " + OrderByClient.Number;// + " - " + OrderByClient.DisplayNameShort;
                    if (OrderByClient.Address != null)
                        del = del + " - " + OrderByClient.Address.Mobile;
                    //del = del + " - " + OrderDate.ToString("dd-MM-yyyy") + "(" + ReportUtility.GetEthCalendarFormated(OrderDate, "/") + ")";
                }
                return del;
            }
            set { SetValue(() => DeliveryDetail, value); }
        }

        [NotMapped]
        [DisplayName("No. of Lines")]
        public int CountLines
        {
            get
            {
                return DeliveryLines.Count(l => l.Enabled && l.DeliveryType == DeliveryLineRouteTypes.Delivering);
            }
            set { SetValue(() => CountLines, value); }
        }

        [NotMapped]
        [DisplayName("No. of Lines")]
        public int CountLinesAll
        {
            get
            {
                return DeliveryLines.Count(l => l.Enabled);
            }
            set { SetValue(() => CountLinesAll, value); }
        }

        [NotMapped]
        [DisplayName("No. of Lines")]
        public int CountMessages
        {
            get
            {
                return DeliveryLines.Sum(l => l.CountMessages);
            }
            set { SetValue(() => CountMessages, value); }
        }

        [NotMapped]
        [DisplayName("No. of Lines")]
        public string CountLinesString
        {
            get
            {
                return "(" + CountLines.ToString(CultureInfo.InvariantCulture) + "-" + CountMessages.ToString(CultureInfo.InvariantCulture) + ") delivery";
            }
            set { SetValue(() => CountLinesString, value); }
        }

        [NotMapped]
        public string OrderDateString
        {
            get
            {
                return OrderDate.ToString("dd-MM-yyyy") + "(" + ReportUtility.GetEthCalendarFormated(OrderDate, "/") + ") - " + OrderDate.ToShortTimeString();
            }
            set { SetValue(() => OrderDateString, value); }
        }

        public ICollection<DeliveryLineDTO> DeliveryLines
        {
            get { return GetValue(() => DeliveryLines); }
            set { SetValue(() => DeliveryLines, value); }
        }
    }
}
