using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEX.Core.Models
{
    public class StorageBinDTO : CommonFieldsA
    {
        public StorageBinDTO()
        {
            MaxQtyPerBox = 0;
            IsActive = true;
        }

        [ForeignKey("Warehouse")]
        public int WarehouseId { get; set; }
        public WarehouseDTO Warehouse
        {
            get { return GetValue(() => Warehouse); }
            set { SetValue(() => Warehouse, value); }
        }
        
        [Required]
        public string Shelve
        {
            get { return GetValue(() => Shelve); }
            set { SetValue(() => Shelve, value); }
        }

        [Required]
        public string BoxNumber
        {
            get { return GetValue(() => BoxNumber); }
            set { SetValue(() => BoxNumber, value); }
        }

        public bool IsActive
        {
            get { return GetValue(() => IsActive); }
            set { SetValue(() => IsActive, value); }
        }

        public int MaxQtyPerBox
        {
            get { return GetValue(() => MaxQtyPerBox); }
            set { SetValue(() => MaxQtyPerBox, value); }
        }

        [NotMapped]
        public string ShelveBoxNumber
        {
            get { return "SH_" + Shelve + "-N_" + BoxNumber; }
            set { SetValue(() => ShelveBoxNumber, value); }
        }

        //[Required]
        //public string Corridor
        //{
        //    get { return GetValue(() => Corridor); }
        //    set { SetValue(() => Corridor, value); }
        //}

        //public string Height
        //{
        //    get { return GetValue(() => Height); }
        //    set { SetValue(() => Height, value); }
        //}

    }
}