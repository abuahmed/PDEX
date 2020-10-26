using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace PDEX.Core.Models
{
    public class WarehouseDTO : CommonFieldsC
    {
        public WarehouseDTO()
        {
            IsActive = true;
            NoOfShelves = 1;
            NoOfBoxes = 10;
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
                    pref = pref.Substring(1);
                }
                return "ST" + pref;
            }
            set { SetValue(() => Number, value); }
        }
        
        public bool IsActive
        {
            get { return GetValue(() => IsActive); }
            set { SetValue(() => IsActive, value); }
        }

        [Required]
        [Range(1,20)]
        public int NoOfShelves
        {
            get { return GetValue(() => NoOfShelves); }
            set { SetValue(() => NoOfShelves, value); }
        }

        [Required]
        [Range(1, 100)]
        public int NoOfBoxes
        {
            get { return GetValue(() => NoOfBoxes); }
            set { SetValue(() => NoOfBoxes, value); }
        }
    }
}