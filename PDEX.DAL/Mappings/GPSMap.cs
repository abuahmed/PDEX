using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class GPSMap : EntityTypeConfiguration<GPSDTO>
    {
        public GPSMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("GPSData");//GPSDetails
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
            HasRequired(t => t.DeliveryRoute)
                .WithMany(t => t.GPSData)
                .HasForeignKey(t => new { t.DeliveryRouteId });
        }
    }
}