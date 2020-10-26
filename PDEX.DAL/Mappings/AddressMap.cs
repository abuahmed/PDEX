using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class AddressMap : EntityTypeConfiguration<AddressDTO>
    {
        public AddressMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Addresses");
            Property(t => t.Id).HasColumnName("Id");
        }
    }
}
