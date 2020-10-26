using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PDEX.Core.Models
{
    public class TenderDTO : CommonFieldsA
    {
        [Required]
        [DisplayName("Tender No.")]
        [MaxLength(10, ErrorMessage = "Exceeded 10 letters")]
        public string TenderNumber
        {
            get { return GetValue(() => TenderNumber); }
            set { SetValue(() => TenderNumber, value); }
        }

        [ForeignKey("PublishedOn")]
        public int? PublishedOnId { get; set; }
        public CategoryDTO PublishedOn
        {
            get { return GetValue(() => PublishedOn); }
            set { SetValue(() => PublishedOn, value); }
        }

        public string PublishedAddress
        {
            get { return GetValue(() => PublishedAddress); }
            set { SetValue(() => PublishedAddress, value); }
        }

        public DateTime? PostOnDate
        {
            get { return GetValue(() => PostOnDate); }
            set { SetValue(() => PostOnDate, value); }
        }

        public DateTime BidClosingDate
        {
            get { return GetValue(() => BidClosingDate); }
            set { SetValue(() => BidClosingDate, value); }
        }

        public DateTime? BidOpenningDate
        {
            get { return GetValue(() => BidOpenningDate); }
            set { SetValue(() => BidOpenningDate, value); }
        }

        public decimal BidDocumentPrice
        {
            get { return GetValue(() => BidDocumentPrice); }
            set { SetValue(() => BidDocumentPrice, value); }
        }

        public decimal BidBondPrice
        {
            get { return GetValue(() => BidBondPrice); }
            set { SetValue(() => BidBondPrice, value); }
        }
        
        [Required]
        public string CompanyName
        {
            get { return GetValue(() => CompanyName); }
            set { SetValue(() => CompanyName, value); }
        }

        [ForeignKey("CompanyAddress")]
        public int? CompanyAddressId { get; set; }
        public AddressDTO CompanyAddress
        {
            get { return GetValue(() => CompanyAddress); }
            set { SetValue(() => CompanyAddress, value); }
        }

        public ICollection<TenderItemDTO> TenderItems
        {
            get { return GetValue(() => TenderItems); }
            set { SetValue(() => TenderItems, value); }
        }
    }
}