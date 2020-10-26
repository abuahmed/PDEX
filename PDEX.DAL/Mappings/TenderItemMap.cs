using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class TenderItemMap : EntityTypeConfiguration<TenderItemDTO>
    {
        public TenderItemMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Table & Column Mappings
            ToTable("TenderItems");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
            HasRequired(t => t.Tender)
                .WithMany(t => t.TenderItems)
                .HasForeignKey(t => new { t.TenderId });

            HasOptional(t => t.TenderItemCategory)
                .WithMany()
                .HasForeignKey(t => new { t.TenderItemCategoryId });

        }
    }
}