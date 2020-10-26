using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEX.Core.Models
{
    public class GPSDTO : CommonFieldsA
    {
        [ForeignKey("DeliveryRoute")]
        public int DeliveryRouteId { get; set; }
        public DeliveryRouteDTO DeliveryRoute
        {
            get { return GetValue(() => DeliveryRoute); }
            set { SetValue(() => DeliveryRoute, value); }
        }

        public DateTime StartTime //may be ignition start time
        {
            get { return GetValue(() => StartTime); }
            set { SetValue(() => StartTime, value); }
        }
        public double StartLatitude 
        {
            get { return GetValue(() => StartLatitude); }
            set { SetValue(() => StartLatitude, value); }
        }
        public double StartLongitude 
        {
            get { return GetValue(() => StartLongitude); }
            set { SetValue(() => StartLongitude, value); }
        }

        public DateTime EndTime //may be ignition stop time
        {
            get { return GetValue(() => EndTime); }
            set { SetValue(() => EndTime, value); }
        }
        public double EndLatitude 
        {
            get { return GetValue(() => EndLatitude); }
            set { SetValue(() => EndLatitude, value); }
        }
        public double EndLongitude 
        {
            get { return GetValue(() => EndLongitude); }
            set { SetValue(() => EndLongitude, value); }
        }

        //public TimeSpan Duration
        //{
        //    get { return EndTime - StartTime; }
        //    set { SetValue(() => Duration, value); }
        //}
        public decimal Distance
        {
            get { return GetValue(() => Distance); }
            set { SetValue(() => Distance, value); }
        }
        
        public int? DeliveredTimeInMinutes
        {
            get { return GetValue(() => DeliveredTimeInMinutes); }
            set { SetValue(() => DeliveredTimeInMinutes, value); }
        }

        public int? EstimatedToAcceptTimeInMinutes
        {
            get { return GetValue(() => EstimatedToAcceptTimeInMinutes); }
            set { SetValue(() => EstimatedToAcceptTimeInMinutes, value); }
        }

        public int? EstimatedToDeliverTimeInMinutes
        {
            get { return GetValue(() => EstimatedToDeliverTimeInMinutes); }
            set { SetValue(() => EstimatedToDeliverTimeInMinutes, value); }
        }
        //and many many others import from excel
    }
}