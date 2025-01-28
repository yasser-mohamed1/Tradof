using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tradof.Data.Entities;

namespace Tradof.EntityFramework.Configurations
{
    internal class FreelancerConfiguration : IEntityTypeConfiguration<Freelancer>
    {
        public void Configure(EntityTypeBuilder<Freelancer> builder)
        {

            builder.HasOne(f => f.User)
                      .WithOne()
                      .HasForeignKey<Freelancer>(f => f.UserId);

            builder.HasOne(f => f.Specialization)
                  .WithMany(s => s.Freelancers)
                  .HasForeignKey(f => f.SpecializationId);

            builder.HasMany(f => f.Projects)
                  .WithOne(p => p.Freelancer)
                  .HasForeignKey(p => p.FreelancerId);

            builder.HasMany(f => f.FreelancerLanguagesPairs)
                  .WithOne(fl => fl.Freelancer)
                  .HasForeignKey(fl => fl.FreelancerId);

            builder.HasMany(f => f.Proposals)
                  .WithOne(pr => pr.Freelancer)
                  .HasForeignKey(pr => pr.FreelancerId);

        }
    }
}
