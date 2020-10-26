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
    public class DeliveryLineDTO : CommonFieldsA
    {
        public DeliveryLineDTO()
        {
            DeliverySuggestion = DeliverySuggestionTypes.NoSuggestion;
            ReceiverSecretCode = CommonUtility.GetSecretCode();
        }

        [ForeignKey("DeliveryHeader")]
        public int DeliveryHeaderId { get; set; }
        public DeliveryHeaderDTO DeliveryHeader
        {
            get { return GetValue(() => DeliveryHeader); }
            set { SetValue(() => DeliveryHeader, value); }
        }

        public DeliveryLineRouteTypes DeliveryType
        {
            get { return GetValue(() => DeliveryType); }
            set { SetValue(() => DeliveryType, value); }
        }

        [DisplayName("Urgency(Hours)")]
        public int UrgencyInHours
        {
            get { return GetValue(() => UrgencyInHours); }
            set { SetValue(() => UrgencyInHours, value); }
        }

        [NotMapped]
        [DisplayName("Delivery Line No.")]
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
                    pref=pref.Substring(1);
                }
                return "L" + pref;
            }
            set { SetValue(() => Number, value); }
        }
        
        #region Begining
        [ForeignKey("FromClient")]
        public int? FromClientId { get; set; }
        public ClientDTO FromClient
        {
            get { return GetValue(() => FromClient); }
            set { SetValue(() => FromClient, value); }
        }

        [ForeignKey("FromAddress")]
        public int? FromAddressId { get; set; }
        public AddressDTO FromAddress
        {
            get { return GetValue(() => FromAddress); }
            set { SetValue(() => FromAddress, value); }
        } 
        #endregion
        
        #region Destination
        [ForeignKey("ToClient")]
        public int? ToClientId { get; set; }
        public ClientDTO ToClient
        {
            get { return GetValue(() => ToClient); }
            set { SetValue(() => ToClient, value); }
        }

        [ForeignKey("ToStaff")]
        public int? ToStaffId { get; set; }
        public StaffDTO ToStaff
        {
            get { return GetValue(() => ToStaff); }
            set { SetValue(() => ToStaff, value); }
        }

        [ForeignKey("ToAddress")]
        public int? ToAddressId { get; set; }
        public AddressDTO ToAddress
        {
            get { return GetValue(() => ToAddress); }
            set { SetValue(() => ToAddress, value); }
        } 
        #endregion
        
        [DisplayName("Estimated Delivery(Hours)")]
        public int EstimatedDeliveryInHours
        {
            get { return GetValue(() => EstimatedDeliveryInHours); }
            set { SetValue(() => EstimatedDeliveryInHours, value); }
        }

        [DisplayName("Delivered Date and Time")]
        public DateTime? DeliveredTime
        {
            get { return GetValue(() => DeliveredTime); }
            set { SetValue(() => DeliveredTime, value); }
        }

        public DeliverySuggestionTypes DeliverySuggestion
        {
            get { return GetValue(() => DeliverySuggestion); }
            set { SetValue(() => DeliverySuggestion, value); }
        }

        [DisplayName("Receiver Secret Code")]
        [MaxLength(5, ErrorMessage = "Exceeded 5 letters")]
        public string ReceiverSecretCode
        {
            get { return GetValue(() => ReceiverSecretCode); }
            set { SetValue(() => ReceiverSecretCode, value); }
        }
        
        [NotMapped]
        [DisplayName("No. of Messages")]
        public int CountMessages
        {
            get
            {
                if (Messages != null) return Messages.Count(l => l.Enabled);
                return 0;
            }
            set { SetValue(() => CountMessages, value); }
        }
        
        [NotMapped]
        [DisplayName("No. of Messages")]
        public string CountMessagesString
        {
            get
            {
                return "(" + CountMessages.ToString(CultureInfo.InvariantCulture) + ") message(s)";
            }
            set { SetValue(() => CountMessagesString, value); }
        }

        public ICollection<MessageDTO> Messages
        {
            get { return GetValue(() => Messages); }
            set { SetValue(() => Messages, value); }
        }
        public ICollection<DeliveryRouteDTO> DeliveryRoutes
        {
            get { return GetValue(() => DeliveryRoutes); }
            set
            {
                SetValue(() => DeliveryRoutes, value);
            }
        }
        public ICollection<PaymentDTO> PaymentsandExpenses
        {
            get { return GetValue(() => PaymentsandExpenses); }
            set { SetValue(() => PaymentsandExpenses, value); }
        }
    }
}