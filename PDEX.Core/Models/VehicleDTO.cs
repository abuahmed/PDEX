using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PDEX.Core.CustomValidationAttributes;
using PDEX.Core.Enumerations;

namespace PDEX.Core.Models
{
    public class VehicleDTO : CommonFieldsA
    {
        public VehicleDTO()
        {
            Type = VehicleTypes.Motor;
        }

        public VehicleTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }

        [MaxLength(25, ErrorMessage = "exceeded 8 letters")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "contains invalid letters")]
        public string Number
        {
            get { return GetValue(() => Number); }
            set { SetValue(() => Number, value); }
        }

        [Required]
        [MaxLength(8, ErrorMessage = "exceeded 8 letters")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "contains invalid letters")]
        public string PlateNumber
        {
            get { return GetValue(() => PlateNumber); }
            set { SetValue(() => PlateNumber, value); }
        }

        public string MotorNumber
        {
            get { return GetValue(() => MotorNumber); }
            set { SetValue(() => MotorNumber, value); }
        }

        public string ChansiNumber
        {
            get { return GetValue(() => ChansiNumber); }
            set { SetValue(() => ChansiNumber, value); }
        }

        [MaxLength(50, ErrorMessage = "exceeded 50 letters")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "contains invalid letters")]
        public string GPSName
        {
            get { return GetValue(() => GPSName); }
            set { SetValue(() => GPSName, value); }
        }

        [MaxLength(50, ErrorMessage = "exceeded 50 letters")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "contains invalid letters")]
        public string NickName
        {
            get { return GetValue(() => NickName); }
            set { SetValue(() => NickName, value); }
        }

        public string Remarks
        {
            get { return GetValue(() => Remarks); }
            set { SetValue(() => Remarks, value); }
        }

        public bool IsActive
        {
            get { return GetValue(() => IsActive); }
            set { SetValue(() => IsActive, value); }
        }

        [NotMapped]
        public string VehicleDetail
        {
            get
            {
                var clDet = PlateNumber + " - " + NickName;
                if (AssignedDriver != null)
                    clDet = clDet + " - " + AssignedDriver.StaffDetail;
                return clDet;
            }
            set { SetValue(() => VehicleDetail, value); }
        }

        [ForeignKey("AssignedDriver")]
        public int? AssignedDriverId { get; set; }
        public StaffDTO AssignedDriver
        {
            get { return GetValue(() => AssignedDriver); }
            set { SetValue(() => AssignedDriver, value); }
        }

        [DisplayName("Assigned On Date")]
        public DateTime? AssignedOnDate
        {
            get { return GetValue(() => AssignedOnDate); }
            set { SetValue(() => AssignedOnDate, value); }
        }
    }
}