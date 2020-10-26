using System.ComponentModel;

namespace PDEX.Core
{
    public class CommonFieldsD : CommonFieldsC
    {
        [DisplayName("Contact Name")]
        public string ContactName
        {
            get { return GetValue(() => ContactName); }
            set { SetValue(() => ContactName, value); }
        }
        [DisplayName("Contact Title")]
        public string ContactTitle
        {
            get { return GetValue(() => ContactTitle); }
            set { SetValue(() => ContactTitle, value); }
        }
    }
}