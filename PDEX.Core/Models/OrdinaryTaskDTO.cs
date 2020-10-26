using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEX.Core.Models
{
    //public class OrdinaryTaskDTO : CommonFieldsA
    //{
    //    [ForeignKey("Client")]
    //    public int? ClientId { get; set; }
    //    public ClientDTO Client
    //    {
    //        get { return GetValue(() => Client); }
    //        set { SetValue(() => Client, value); }
    //    }

    //    [ForeignKey("OrdinaryTaskProcessCategory")]
    //    public int? OrdinaryTaskProcessCategoryId { get; set; }
    //    public CategoryDTO OrdinaryTaskProcessCategory
    //    {
    //        get { return GetValue(() => OrdinaryTaskProcessCategory); }
    //        set { SetValue(() => OrdinaryTaskProcessCategory, value); }
    //    }

    //    public string CompanyName
    //    {
    //        get { return GetValue(() => CompanyName); }
    //        set { SetValue(() => CompanyName, value); }
    //    }

    //    [ForeignKey("Address")]
    //    public int? AddressId { get; set; }
    //    public AddressDTO Address
    //    {
    //        get { return GetValue(() => Address); }
    //        set { SetValue(() => Address, value); }
    //    }

    //    public ICollection<TaskProcessDTO> OrdinaryTaskProcesses
    //    {
    //        get { return GetValue(() => OrdinaryTaskProcesses); }
    //        set { SetValue(() => OrdinaryTaskProcesses, value); }
    //    }
    //    //Ordinary Task Status
    //}
}