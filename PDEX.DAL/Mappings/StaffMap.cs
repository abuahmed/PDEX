using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class StaffMap : EntityTypeConfiguration<StaffDTO>
    {
        public StaffMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.DisplayName)
                .IsRequired();

            // Table & Column Mappings
            ToTable("Staffs");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
            HasOptional(t => t.Address)
                .WithMany()
                .HasForeignKey(t => new { t.AddressId });

            HasOptional(t => t.ContactPerson)
                .WithMany()
                .HasForeignKey(t => new { t.ContactPersonId });
        }
    }
}