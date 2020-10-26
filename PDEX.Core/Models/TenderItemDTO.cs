using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEX.Core.Models
{
    public class TenderItemDTO : CommonFieldsA
    {
        [ForeignKey("Tender")]
        public int TenderId { get; set; }
        public TenderDTO Tender
        {
            get { return GetValue(() => Tender); }
            set { SetValue(() => Tender, value); }
        }

        [ForeignKey("TenderItemCategory")]
        public int? TenderItemCategoryId { get; set; }
        public CategoryDTO TenderItemCategory
        {
            get { return GetValue(() => TenderItemCategory); }
            set { SetValue(() => TenderItemCategory, value); }
        }

        public ICollection<TaskProcessDTO> TenderTasks
        {
            get { return GetValue(() => TenderTasks); }
            set { SetValue(() => TenderTasks, value); }
        }
    }
}