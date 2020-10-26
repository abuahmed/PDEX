using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEX.Core.Models
{
    //public class TenderTaskDTO : CommonFieldsA
    //{
    //    [ForeignKey("Client")]
    //    public int? ClientId { get; set; }
    //    public ClientDTO Client
    //    {
    //        get { return GetValue(() => Client); }
    //        set { SetValue(() => Client, value); }
    //    }
        
    //    [ForeignKey("TenderItem")]
    //    public int TenderItemId { get; set; }
    //    public TenderItemDTO TenderItem
    //    {
    //        get { return GetValue(() => TenderItem); }
    //        set { SetValue(() => TenderItem, value); }
    //    }

    //    [ForeignKey("TenderItemProcessCategory")]
    //    public int? TenderItemProcessCategoryId { get; set; }
    //    public CategoryDTO TenderItemProcessCategory
    //    {
    //        get { return GetValue(() => TenderItemProcessCategory); }
    //        set { SetValue(() => TenderItemProcessCategory, value); }
    //    }

    //    public ICollection<TaskProcessDTO> TenderProcesses
    //    {
    //        get { return GetValue(() => TenderProcesses); }
    //        set { SetValue(() => TenderProcesses, value); }
    //    }
    //    //Tender Task Status
    //}
}