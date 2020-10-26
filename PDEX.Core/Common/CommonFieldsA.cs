using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using PDEX.Core.Models;

namespace PDEX.Core
{
    public class CommonFieldsA : EntityBase
    {
        [NotMapped]
        [DisplayName("No.")]
        public int SerialNumber { get; set; }

        [NotMapped]
        public string DateRecordCreatedString
        {
            get
            {
                if (DateRecordCreated != null)
                {
                    var det = DateRecordCreated.Value;
                    return det.ToString("dd-MM-yyyy") + "(" + ReportUtility.GetEthCalendarFormated(det, "/") + ")";
                }
                return "";
            }
            set { SetValue(() => DateRecordCreatedString, value); }
        }

        [NotMapped]
        public string DateLastModifiedString
        {
            get
            {
                if (DateLastModified != null)
                {
                    var det = DateLastModified.Value;
                    return det.ToString("dd-MM-yyyy") + "(" + ReportUtility.GetEthCalendarFormated(det, "/") + ")";
                }
                return "";
            }
            set { SetValue(() => DateLastModifiedString, value); }
        }
    }

}
