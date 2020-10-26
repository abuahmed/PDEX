using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class TaskProcessMap : EntityTypeConfiguration<TaskProcessDTO>
    {
        public TaskProcessMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Table & Column Mappings
            ToTable("TaskProcesses");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
            HasOptional(t => t.Client)
                .WithMany(t => t.TaskProcesses)
                .HasForeignKey(t => new { t.ClientId });

            HasOptional(t => t.TenderItem)
                .WithMany(t => t.TenderTasks)
                .HasForeignKey(t => new { t.TenderItemId });

            HasOptional(t => t.CompanyAddress)
                .WithMany()
                .HasForeignKey(t => new { t.CompanyAddressId });

            HasOptional(t => t.TaskProcessCategory)
                .WithMany()
                .HasForeignKey(t => new { t.TaskProcessCategoryId });

            HasOptional(t => t.AssignedTo)
               .WithMany(t => t.TaskProcesses)
               .HasForeignKey(t => new { t.AssignedToId });
        }
    }
}