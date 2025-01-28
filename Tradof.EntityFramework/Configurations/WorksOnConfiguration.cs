using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tradof.Data.Entities;

namespace Tradof.EntityFramework.Configurations
{
    internal class WorksOnConfiguration : IEntityTypeConfiguration<WorksOn>
    {
        public void Configure(EntityTypeBuilder<WorksOn> builder)
        {


            builder.HasOne(wo => wo.Freelancer)
                      .WithMany(f => f.WorksOns)
                      .HasForeignKey(wo => wo.FreelancerId);

            builder.HasOne(wo => wo.Project)
                      .WithOne();

            builder.HasOne(w => w.Project)
                .WithMany()
                .HasForeignKey(w => w.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
