using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tradof.Data.Entities;

namespace Tradof.EntityFramework.Configurations
{
    internal class ProjectPaymentConfiguration : IEntityTypeConfiguration<ProjectPayment>
    {
        public void Configure(EntityTypeBuilder<ProjectPayment> builder)
        {
            builder.HasOne(pp => pp.Project)
                .WithMany()
                .HasForeignKey(pp => pp.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
