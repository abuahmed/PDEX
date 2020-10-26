using System.ComponentModel.DataAnnotations.Schema;
using PDEX.Core.Models;

namespace PDEX.Core
{
    public class CommonFieldsC : CommonFieldsB
    {
        [ForeignKey("Address")]
        public int? AddressId { get; set; }
        public virtual AddressDTO Address
        {
            get { return GetValue(() => Address); }
            set { SetValue(() => Address, value); }
        }
    }
}
