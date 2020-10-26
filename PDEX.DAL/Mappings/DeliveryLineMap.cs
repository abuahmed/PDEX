using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class DeliveryLineMap : EntityTypeConfiguration<DeliveryLineDTO>
    {
        public DeliveryLineMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.DeliveryHeaderId)
                .IsRequired();

            // Table & Column Mappings
            ToTable("DeliveryLines");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
            HasRequired(t => t.DeliveryHeader)
               .WithMany(t => t.DeliveryLines)
               .HasForeignKey(t => new { t.DeliveryHeaderId });

            HasOptional(t => t.FromClient)
               .WithMany(t => t.Sends)
               .HasForeignKey(t => new { t.FromClientId });

            HasOptional(t => t.FromAddress)
                .WithMany()
                .HasForeignKey(t => new { t.FromAddressId });

            HasOptional(t => t.ToAddress)
                .WithMany()
                .HasForeignKey(t => new { t.ToAddressId });

            HasOptional(t => t.ToClient)
                .WithMany(t => t.Receives)
                .HasForeignKey(t => new { t.ToClientId });

            HasOptional(t => t.ToStaff)
                .WithMany(t => t.StaffReceives)
                .HasForeignKey(t => new { t.ToStaffId });
        }
    }
}