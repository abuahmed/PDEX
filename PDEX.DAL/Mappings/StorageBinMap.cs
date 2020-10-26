using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class StorageBinMap : EntityTypeConfiguration<StorageBinDTO>
    {
        public StorageBinMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("StorageBins");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
            HasRequired(t => t.Warehouse)
                .WithMany()
                .HasForeignKey(t => new { t.WarehouseId });
        }
    }
}