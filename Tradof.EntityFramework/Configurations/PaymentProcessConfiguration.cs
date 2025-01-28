using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tradof.Data.Entities;

namespace Tradof.EntityFramework.Configurations
{
    internal class PaymentProcessConfiguration : IEntityTypeConfiguration<PaymentProcess>
    {
        public void Configure(EntityTypeBuilder<PaymentProcess> builder)
        {
            builder
                .HasOne(pp => pp.Freelancer)
                .WithMany()
                .HasForeignKey(pp => pp.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(pp => pp.Payment)
                .WithMany()
                .HasForeignKey(pp => pp.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
