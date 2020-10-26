using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class DocumentMap : EntityTypeConfiguration<DocumentDTO>
    {
        public DocumentMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Documents");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
            HasRequired(t => t.Client)
                .WithMany(t => t.Documents)
                .HasForeignKey(t => new { t.ClientId });
        }
    }
}