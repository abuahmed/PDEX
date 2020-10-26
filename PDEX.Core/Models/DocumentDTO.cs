using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEX.Core.Models
{
    public class DocumentDTO : CommonFieldsA
    {
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public ClientDTO Client
        {
            get { return GetValue(() => Client); }
            set { SetValue(() => Client, value); }
        }

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        public CategoryDTO Category
        {
            get { return GetValue(() => Category); }
            set { SetValue(() => Category, value); }
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
        public int Unit
        {
            get { return GetValue(() => Unit); }
            set { SetValue(() => Unit, value); }
        }

        public string Number
        {
            get { return GetValue(() => Number); }
            set { SetValue(() => Number, value); }
        }

        public string Comments
        {
            get { return GetValue(() => Comments); }
            set { SetValue(() => Comments, value); }
        }

        [ForeignKey("StorageBin")]
        public int? StorageBinId { get; set; }
        public StorageBinDTO StorageBin
        {
            get { return GetValue(() => StorageBin); }
            set { SetValue(() => StorageBin, value); }
        }

        [NotMapped]
        public string DocumentDetail
        {
            get
            {
                return Number + "-" + Description;
            }
            set { SetValue(() => DocumentDetail, value); }
        }

        [NotMapped]
        public string StorageDetail
        {
            get
            {
                string det = "Not Known";
                if (Client != null)
                    det = Client.Number;
                if (StorageBin != null)
                    det = StorageBin.ShelveBoxNumber;
                return det;

            }
            set { SetValue(() => StorageDetail, value); }
        }
        
    }
}