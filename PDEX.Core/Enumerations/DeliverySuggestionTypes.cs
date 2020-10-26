using System.ComponentModel;

namespace PDEX.Core.Enumerations
{
    public enum DeliverySuggestionTypes
    {
        All,
        [Description("No Suggestion")]
        NoSuggestion,
        [Description("Very Good")]
        VeryGood,
        [Description("Good")]
        Good,
        [Description("Fair")]
        Fair,
        [Description("Poor")]
        Poor
    }
}