using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PDEX.Core.Enumerations;

namespace PDEX.Core.Models
{
    public class DeliveryRouteDTO : CommonFieldsA
    {
        public DeliveryRouteDTO()
        {
            ReceiverSecretCode = CommonUtility.GetSecretCode();
        }

        [ForeignKey("DeliveryLine")]
        public int DeliveryLineId { get; set; }
        public DeliveryLineDTO DeliveryLine
        {
            get { return GetValue(() => DeliveryLine); }
            set { SetValue(() => DeliveryLine, value); }
        }

        public DeliveryLineRouteTypes DeliveryType
        {
            get { return GetValue(() => DeliveryType); }
            set { SetValue(() => DeliveryType, value); }
        }

        public DeliveryMethods DeliveryMethod
        {
            get { return GetValue(() => DeliveryMethod); }
            set { SetValue(() => DeliveryMethod, value); }
        }

        [ForeignKey("AssignedToStaff")]
        public int? AssignedToStaffId { get; set; }
        public StaffDTO AssignedToStaff
        {
            get { return GetValue(() => AssignedToStaff); }
            set { SetValue(() => AssignedToStaff, value); }
        }

        [ForeignKey("Vehicle")]
        public int? VehicleId { get; set; }
        public VehicleDTO Vehicle
        {
            get { return GetValue(() => Vehicle); }
            set { SetValue(() => Vehicle, value); }
        }
        
        //Same as Start Time
        [DisplayName("Started Date and Time")]
        public DateTime? StartedTime
        {
            get { return GetValue(() => StartedTime); }
            set { SetValue(() => StartedTime, value); }
        }

        [ForeignKey("FromAddress")]
        public int? FromAddressId { get; set; }
        public AddressDTO FromAddress
        {
            get { return GetValue(() => FromAddress); }
            set { SetValue(() => FromAddress, value); }
        }

        [ForeignKey("Receiver")]
        public int? ReceiverId { get; set; }
        public ClientDTO Receiver
        {
            get { return GetValue(() => Receiver); }
            set { SetValue(() => Receiver, value); }
        }

        //For the moment delivery can only be to ClientDTO Receiver
        [ForeignKey("StaffReceiver")]
        public int? StaffReceiverId { get; set; }
        public StaffDTO StaffReceiver
        {
            get { return GetValue(() => StaffReceiver); }
            set { SetValue(() => StaffReceiver, value); }
        }

        [ForeignKey("ToAddress")]
        public int? ToAddressId { get; set; }
        public AddressDTO ToAddress
        {
            get { return GetValue(() => ToAddress); }
            set { SetValue(() => ToAddress, value); }
        }

        //End Time
        [DisplayName("Ended Date and Time")]
        public DateTime? EndedTime
        {
            get { return GetValue(() => EndedTime); }
            set { SetValue(() => EndedTime, value); }
        } 
        
        [DisplayName("Receiver Secret Code")]
        [MaxLength(5, ErrorMessage = "Exceeded 5 letters")]
        public string ReceiverSecretCode
        {
            get { return GetValue(() => ReceiverSecretCode); }
            set { SetValue(() => ReceiverSecretCode, value); }
        }

        public ICollection<GPSDTO> GPSData
        {
            get { return GetValue(() => GPSData); }
            set { SetValue(() => GPSData, value); }
        }
    }
}