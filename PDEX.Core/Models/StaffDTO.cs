using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using PDEX.Core.CustomValidationAttributes;
using PDEX.Core.Enumerations;
using PDEX.Core.Extensions;

namespace PDEX.Core.Models
{
    public class StaffDTO : CommonFieldsC
    {
        public StaffDTO()
        {
            Type = StaffTypes.OfficeStaff;
            EducationLevel = EducationLevelTypes.Elementary;
            IsActive = true;
            DateOfBirth = null;
            Sex = Sex.Male;
        }

        public StaffTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }

        public EducationLevelTypes EducationLevel
        {
            get { return GetValue(() => EducationLevel); }
            set { SetValue(() => EducationLevel, value); }
        }

        [StringLength(50)]
        public string FieldOfStudy
        {
            get { return GetValue(() => FieldOfStudy); }
            set { SetValue(() => FieldOfStudy, value); }
        }

        [MaxLength(50, ErrorMessage = "exceeded 50 letters")]
        public string Skill
        {
            get { return GetValue(() => Skill); }
            set { SetValue(() => Skill, value); }
        }

        [Required]
        public Sex Sex
        {
            get { return GetValue(() => Sex); }
            set { SetValue(() => Sex, value); }
        }

        public DateTime? DateOfBirth
        {
            get { return GetValue(() => DateOfBirth); }
            set
            {
                SetValue(() => DateOfBirth, value);
            }
        }

        [NotMapped]
        public int Age
        {
            get
            {
                if (DateOfBirth != null)
                {
                    int age = DateTime.Now.Subtract(DateOfBirth.Value).Days;
                    age = (int)(age / 365.25);
                    return age;
                }
                return 0;
            }
            set { SetValue(() => Age, value); }
        }

        [NotMapped]
        public string BirthDateEc
        {
            get
            {
                if (DateOfBirth != null)
                {
                    var ec = ReportUtility.GetEthCalendarFormated(DateOfBirth.Value, "-");
                    return ec;
                }
                return "";
            }
            set { SetValue(() => BirthDateEc, value); }
        }

        [MaxLength(50, ErrorMessage = "exceeded 50 letters")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "contains invalid letters")]
        public string NickName
        {
            get { return GetValue(() => NickName); }
            set { SetValue(() => NickName, value); }
        }

        [MaxLength(50, ErrorMessage = "Code exceeded 50 letters")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "Code contains invalid letters")]
        public string Code
        {
            get { return GetValue(() => Code); }
            set { SetValue(() => Code, value); }
        }

        public bool IsActive
        {
            get { return GetValue(() => IsActive); }
            set { SetValue(() => IsActive, value); }
        }

        [MaxLength]
        public byte[] ShortPhoto
        {
            get { return GetValue(() => ShortPhoto); }
            set { SetValue(() => ShortPhoto, value); }
        }

        [NotMapped]
        public string StaffDetail
        {
            get
            {
                var clDet = DisplayName + " - " + Code;
                if (Address != null)
                    clDet = clDet + " - " + Address.Mobile;
                return clDet;
            }
            set { SetValue(() => StaffDetail, value); }
        }

        [NotMapped]
        public string LevelString
        {
            get
            {
                return EnumUtil.GetEnumDesc(EducationLevel);
            }
            set { SetValue(() => LevelString, value); }
        }

        [ForeignKey("ContactPerson")]
        public int? ContactPersonId { get; set; }
        public ContactPersonDTO ContactPerson
        {
            get { return GetValue(() => ContactPerson); }
            set { SetValue(() => ContactPerson, value); }
        }

        public ICollection<DeliveryRouteDTO> StaffReceivers
        {
            get { return GetValue(() => StaffReceivers); }
            set { SetValue(() => StaffReceivers, value); }
        }

        public ICollection<DeliveryRouteDTO> AssignedStaffs
        {
            get { return GetValue(() => AssignedStaffs); }
            set { SetValue(() => AssignedStaffs, value); }
        }

        public ICollection<VehicleDTO> AssignedVehicles
        {
            get { return GetValue(() => AssignedVehicles); }
            set { SetValue(() => AssignedVehicles, value); }
        }

        public ICollection<DeliveryLineDTO> StaffReceives
        {
            get { return GetValue(() => StaffReceives); }
            set { SetValue(() => StaffReceives, value); }
        }

        public ICollection<TaskProcessDTO> TaskProcesses
        {
            get { return GetValue(() => TaskProcesses); }
            set { SetValue(() => TaskProcesses, value); }
        }
    }
}