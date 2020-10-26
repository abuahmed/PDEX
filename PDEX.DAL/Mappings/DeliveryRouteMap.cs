using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class DeliveryRouteMap : EntityTypeConfiguration<DeliveryRouteDTO>
    {
        public DeliveryRouteMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("DeliveryRoutes");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
            HasRequired(t => t.DeliveryLine)
                .WithMany(t => t.DeliveryRoutes)
                .HasForeignKey(t => new { t.DeliveryLineId });

            HasOptional(t => t.FromAddress)
               .WithMany()
               .HasForeignKey(t => new { t.FromAddressId });

            HasOptional(t => t.ToAddress)
                .WithMany()
                .HasForeignKey(t => new { t.ToAddressId });

            HasOptional(t => t.Receiver)
                .WithMany(t => t.RouteReceives)
                .HasForeignKey(t => new { t.ReceiverId });

            HasOptional(t => t.AssignedToStaff)
                .WithMany(t => t.AssignedStaffs)
                .HasForeignKey(t => new { t.AssignedToStaffId });

            HasOptional(t => t.StaffReceiver)
                .WithMany(t => t.StaffReceivers)
                .HasForeignKey(t => new { t.StaffReceiverId });
        }
    }
}