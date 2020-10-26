using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PDEX.Core.CustomValidationAttributes;
using PDEX.Core.Enumerations;

namespace PDEX.Core.Models
{
    public class ClientDTO : CommonFieldsC
    {
        public ClientDTO()
        {
            Type = ClientTypes.Organization;
            IsActive = true;
            IsReceiver = false;
        }

        public ClientTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        public CategoryDTO Category
        {
            get { return GetValue(() => Category); }
            set { SetValue(() => Category, value); }
        }

        [MaxLength(50, ErrorMessage = "Code exceeded 50 letters")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "Code contains invalid letters")]
        public string Number
        {
            get { return GetValue(() => Number); }
            set { SetValue(() => Number, value); }
        }

        [Required]
        [MaxLength(50, ErrorMessage = "Code exceeded 50 letters")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "Code contains invalid letters")]
        public string Code
        {
            get { return GetValue(() => Code); }
            set { SetValue(() => Code, value); }
        }

        [Display(Name = "Contact Name")]
        [MaxLength(50, ErrorMessage = "Contact Name exceeded 50 letters")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "Contact Name contains invalid letters")]
        public string ContactName
        {
            get { return GetValue(() => ContactName); }
            set { SetValue(() => ContactName, value); }
        }

        [Display(Name = "Contact Title")]
        [MaxLength(50, ErrorMessage = "Contact Title exceeded 50 letters")]
        [ExcludeChar("/.,!@#$%", ErrorMessage = "Contact Title contains invalid letters")]
        public string ContactTitle
        {
            get { return GetValue(() => ContactTitle); }
            set { SetValue(() => ContactTitle, value); }
        }

        [DisplayName("TIN Number")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Invalid Tin No, Must be 10 digit")]
        public string TinNumber
        {
            get { return GetValue(() => TinNumber); }
            set { SetValue(() => TinNumber, value); }
        }

        [DisplayName("VAT Number")]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Invalid VAT No, Must be 10 digit")]
        public string VatNumber
        {
            get { return GetValue(() => VatNumber); }
            set { SetValue(() => VatNumber, value); }
        }

        public bool IsReceiver
        {
            get { return GetValue(() => IsReceiver); }
            set { SetValue(() => IsReceiver, value); }
        }

        public bool IsActive
        {
            get { return GetValue(() => IsActive); }
            set { SetValue(() => IsActive, value); }
        }

        public bool EmailNotifications
        {
            get { return GetValue(() => EmailNotifications); }
            set { SetValue(() => EmailNotifications, value); }
        }

        public bool SMSNotifications
        {
            get { return GetValue(() => SMSNotifications); }
            set { SetValue(() => SMSNotifications, value); }
        }

        [NotMapped]
        public string ClientDetail
        {
            get
            {
                var clDet = DisplayName + " - " + Number;
                if (Address != null)
                    clDet = clDet + " - " + Address.Mobile + " - " + Address.AlternateMobile + " - " + Address.Telephone;
                return clDet;
            }
            set { SetValue(() => ClientDetail, value); }
        }

        public string NoOfDocuments
        {
            get
            {
                if (Documents != null) return "     " + Documents.Count.ToString();//+ " Documents";
                return "     0";
            }
            set { SetValue(() => NoOfDocuments, value); }
        }

        public ICollection<DeliveryHeaderDTO> Deliveries
        {
            get { return GetValue(() => Deliveries); }
            set { SetValue(() => Deliveries, value); }
        }
        public ICollection<DeliveryLineDTO> Receives
        {
            get { return GetValue(() => Receives); }
            set { SetValue(() => Receives, value); }
        }
        public ICollection<DeliveryLineDTO> Sends
        {
            get { return GetValue(() => Sends); }
            set { SetValue(() => Sends, value); }
        }
        public ICollection<DeliveryRouteDTO> RouteReceives
        {
            get { return GetValue(() => RouteReceives); }
            set { SetValue(() => RouteReceives, value); }
        }
        public ICollection<DocumentDTO> Documents
        {
            get { return GetValue(() => Documents); }
            set { SetValue(() => Documents, value); }
        }
        public ICollection<TaskProcessDTO> TaskProcesses
        {
            get { return GetValue(() => TaskProcesses); }
            set { SetValue(() => TaskProcesses, value); }
        }
    }
}
