using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PDEX.Core
{
    public class CommonFieldsB : CommonFieldsA
    {
        [Required(ErrorMessage = "Name Is Required")]
        [MaxLength(255, ErrorMessage = "Name Exceeded 255 letters")]
        [DisplayName("Name")]
        public string DisplayName
        {
            get { return GetValue(() => DisplayName); }
            set { SetValue(() => DisplayName, value); SetValue(() => DisplayNameShort, value); }
        }

        [DisplayName("Description")]
        public string Description
        {
            get { return GetValue(() => Description); }
            set { SetValue(() => Description, value); }
        }

        //public string DisplayNameAmharic
        //{
        //    get { return GetValue(() => DisplayNameAmharic); }
        //    set { SetValue(() => DisplayNameAmharic, value); }
        //}

        [NotMapped]
        public string DisplayNameShort
        {
            get { return DisplayName != null && DisplayName.Length > 18 ? DisplayName.Substring(0, 15) + "..." : DisplayName; }
            set { SetValue(() => DisplayNameShort, value); }
        }

        [NotMapped]
        public Uri PhotoPath { get; set; }
    }
}
