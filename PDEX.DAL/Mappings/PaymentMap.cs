using System.Data.Entity.ModelConfiguration;
using PDEX.Core.Models;
using PDEX.Core.Models;

namespace PDEX.DAL.Mappings
{
    public class PaymentMap : EntityTypeConfiguration<PaymentDTO>
    {
        public PaymentMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.PaymentDate)
               .IsRequired();

            Property(t => t.Amount)
                .IsRequired();

            Property(t => t.Reason)
                .IsRequired();

            Property(t => t.Type)
                .IsRequired();

            // Table & Column Mappings
            ToTable("Payments");
            Property(t => t.Id).HasColumnName("Id");

            //Relationships
            HasOptional(t => t.DeliveryLine)
                .WithMany(t => t.PaymentsandExpenses)
                .HasForeignKey(t => new { t.DeliveryLineId });
            
            HasOptional(t => t.TaskProcess)
                .WithMany(t => t.PaymentsandExpenses)
                .HasForeignKey(t => t.TaskProcessId);

        }
    }
}
