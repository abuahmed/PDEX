using System.ComponentModel;

namespace PDEX.Core.Enumerations
{
    public enum ProcessStatusTypes
    {
        [Description("Ordered")]
        Ordered,
        [Description("On Process")]
        OnProcess,
        [Description("Completed")]
        Completed,
    }
}