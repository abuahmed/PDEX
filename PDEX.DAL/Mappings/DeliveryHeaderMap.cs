using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class DeliveryHeaderMap : EntityTypeConfiguration<DeliveryHeaderDTO>
    {
        public DeliveryHeaderMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.OrderDate)
                .IsRequired();

            // Table & Column Mappings
            ToTable("DeliveryHeaders");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
            HasOptional(t => t.OrderByClient)
                .WithMany(t=>t.Deliveries)
                .HasForeignKey(t => new { t.OrderByClientId });
        }
    }
}