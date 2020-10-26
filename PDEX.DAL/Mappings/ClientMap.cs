using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class ClientMap : EntityTypeConfiguration<ClientDTO>
    {
        public ClientMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.DisplayName)
                .IsRequired();
            
            // Table & Column Mappings
            ToTable("Clients");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
            HasOptional(t => t.Address)
                .WithMany()
                .HasForeignKey(t => new { t.AddressId });
        }
    }
}