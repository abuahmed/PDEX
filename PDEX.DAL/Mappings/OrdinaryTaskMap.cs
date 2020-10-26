using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    //public class OrdinaryTaskMap : EntityTypeConfiguration<OrdinaryTaskDTO>
    //{
    //    public OrdinaryTaskMap()
    //    {
    //        // Primary Key
    //        HasKey(t => t.Id);

    //        // Table & Column Mappings
    //        ToTable("OrdinaryTasks");
    //        Property(t => t.Id).HasColumnName("Id");

    //        //Relationships
    //        HasOptional(t => t.Client)
    //            .WithMany(t => t.OrdinaryTasks)
    //            .HasForeignKey(t => new { t.ClientId });

    //        HasOptional(t => t.Address)
    //            .WithMany()
    //            .HasForeignKey(t => new { t.AddressId });

    //        HasOptional(t => t.OrdinaryTaskProcessCategory)
    //            .WithMany()
    //            .HasForeignKey(t => new { t.OrdinaryTaskProcessCategoryId });

    //    }
    //}
}