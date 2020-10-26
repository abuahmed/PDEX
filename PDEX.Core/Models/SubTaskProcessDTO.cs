using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace PDEX.Core.Models
{
    //public class SubTaskProcessDTO : CommonTaskFields
    //{
    //    [ForeignKey("TaskProcess")]
    //    public int TaskProcessId { get; set; }
    //    public TaskProcessDTO TaskProcess
    //    {
    //        get { return GetValue(() => TaskProcess); }
    //        set { SetValue(() => TaskProcess, value); }
    //    }

    //    [NotMapped]
    //    [DisplayName("Sub-Process No.")]
    //    [MaxLength(10, ErrorMessage = "Exceeded 10 letters")]
    //    public string Number
    //    {
    //        get
    //        {
    //            var pref = Id.ToString(CultureInfo.InvariantCulture);
    //            if (Id < 1000)
    //            {
    //                var id = Id + 10000;
    //                pref = id.ToString(CultureInfo.InvariantCulture);
    //                pref = pref.Substring(1);
    //            }
    //            return "SP" + pref;
    //        }
    //        set
    //        {
    //            SetValue(() => Number, value);
    //        }
    //    }
    //}
}