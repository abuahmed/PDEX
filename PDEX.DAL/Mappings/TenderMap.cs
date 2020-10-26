using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class TenderMap : EntityTypeConfiguration<TenderDTO>
    {
        public TenderMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.TenderNumber)
                .IsRequired();

            // Table & Column Mappings
            ToTable("Tenders");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
            HasOptional(t => t.PublishedOn)
                .WithMany()
                .HasForeignKey(t => new { t.PublishedOnId });

            HasOptional(t => t.CompanyAddress)
                .WithMany()
                .HasForeignKey(t => new { t.CompanyAddressId });

        }
    }
}