using PDEX.Core.Enumerations;

namespace PDEX.Core.Models
{
    public class CategoryDTO : CommonFieldsB
    {
        public NameTypes NameType
        {
            get { return GetValue(() => NameType); }
            set { SetValue(() => NameType, value); }
        }
    }
}
