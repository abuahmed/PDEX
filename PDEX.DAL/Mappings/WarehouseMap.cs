using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class WarehouseMap : EntityTypeConfiguration<WarehouseDTO>
    {
        public WarehouseMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Warehouses");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
           
        }
    }
}