using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class VehicleMap : EntityTypeConfiguration<VehicleDTO>
    {
        public VehicleMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Vehicles");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
            HasOptional(t => t.AssignedDriver)
                .WithMany(t=>t.AssignedVehicles)
                .HasForeignKey(t => new { t.AssignedDriverId });
        }
    }
}