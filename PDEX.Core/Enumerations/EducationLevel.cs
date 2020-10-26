using System.ComponentModel;

namespace PDEX.Core.Enumerations
{
    public enum EducationLevelTypes
    {
        [Description("Elementary")]
        Elementary = 0,
        [Description("Secondary Level")]
        SecondaryLevel = 1,
        [Description("Vocational Level")]
        VocationalLevel = 2,
        [Description("College/University Graduate")]
        CollegeLevel = 3,
        [Description("Post Graduate")]
        PostGraduateLevel = 4,
        [Description("Non Formal Education")]
        NonFormalEducation = 5
    }
}