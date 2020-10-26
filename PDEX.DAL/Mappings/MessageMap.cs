using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class MessageMap : EntityTypeConfiguration<MessageDTO>
    {
        public MessageMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            // Properties

            // Table & Column Mappings
            ToTable("Messages");
            Property(t => t.Id).HasColumnName("Id");

            //RelationShips
            HasRequired(t => t.DeliveryLine)
              .WithMany(t => t.Messages)
              .HasForeignKey(t => new { t.DeliveryLineId });

            HasOptional(t => t.Category)
               .WithMany()
               .HasForeignKey(t => new { t.CategoryId });

            HasOptional(t => t.UnitOfMeasure)
              .WithMany()
              .HasForeignKey(t => new { t.UnitOfMeasureId });

            HasOptional(t => t.StorageBin)
              .WithMany()
              .HasForeignKey(t => new { t.StorageBinId });
        }
    }
}