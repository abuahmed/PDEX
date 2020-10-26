using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    //public class TenderTaskMap : EntityTypeConfiguration<TenderTaskDTO>
    //{
    //    public TenderTaskMap()
    //    {
    //        // Primary Key
    //        HasKey(t => t.Id);

    //        // Table & Column Mappings
    //        ToTable("TenderTasks");
    //        Property(t => t.Id).HasColumnName("Id");

    //        //Relationships
    //        HasOptional(t => t.Client)
    //            .WithMany(t => t.TenderTasks)
    //            .HasForeignKey(t => new { t.ClientId });

    //        HasRequired(t => t.TenderItem)
    //            .WithMany(t => t.TenderTasks)
    //            .HasForeignKey(t => new { t.TenderItemId });

    //        HasOptional(t => t.TenderItemProcessCategory)
    //            .WithMany()
    //            .HasForeignKey(t => new { t.TenderItemProcessCategoryId });

    //    }
    //}
}