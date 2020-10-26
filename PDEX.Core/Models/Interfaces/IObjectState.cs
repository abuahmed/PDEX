using System.ComponentModel.DataAnnotations.Schema;


namespace PDEX.Core.Models.Interfaces
{
    public interface IObjectState
    {
        [NotMapped]
        ObjectState ObjectState { get; set; }
    }
}
