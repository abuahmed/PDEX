using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace PDEX.Core.Models
{
    public class MessageDTO : CommonFieldsA
    {
        [ForeignKey("DeliveryLine")]
        public int DeliveryLineId { get; set; }
        public DeliveryLineDTO DeliveryLine
        {
            get { return GetValue(() => DeliveryLine); }
            set { SetValue(() => DeliveryLine, value); }
        }

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        public CategoryDTO Category
        {
            get { return GetValue(() => Category); }
            set { SetValue(() => Category, value); }
        }

        [ForeignKey("UnitOfMeasure")]
        public int? UnitOfMeasureId { get; set; }
        public CategoryDTO UnitOfMeasure
        {
            get { return GetValue(() => UnitOfMeasure); }
            set { SetValue(() => UnitOfMeasure, value); }
        }

        [Required]
        public string Description
        {
            get { return GetValue(() => Description); }
            set { SetValue(() => Description, value); }
        }

        [NotMapped]
        public string DescriptionShort
        {
            get { return Description != null && Description.Length > 18 ? Description.Substring(0, 15) + "..." : Description; }
            set { SetValue(() => DescriptionShort, value); }
        }

        [Required]
        public decimal Unit
        {
            get { return GetValue(() => Unit); }
            set { SetValue(() => Unit, value); }
        }

        //Used For barcode and should also be created manually, like from the barcode stickers
        //[Required]
        public string StickerNumber
        {
            get { return GetValue(() => StickerNumber); }
            set { SetValue(() => StickerNumber, value); }
        }

        [NotMapped]
        [DisplayName("Message No.")]
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
                return "M" + pref;
            }
            set { SetValue(() => Number, value); }
        }
        
        public string Comments
        {
            get { return GetValue(() => Comments); }
            set { SetValue(() => Comments, value); }
        }

        //Storage Location...
        [ForeignKey("StorageBin")]
        public int? StorageBinId { get; set; }
        public StorageBinDTO StorageBin
        {
            get { return GetValue(() => StorageBin); }
            set { SetValue(() => StorageBin, value); }
        }

        [NotMapped]
        public string MessageDetail
        {
            get
            {
                return Number + " - " + DescriptionShort;
            }
            set { SetValue(() => MessageDetail, value); }
        }
    }
}