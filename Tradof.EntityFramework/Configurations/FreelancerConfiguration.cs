using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
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

            builder.HasMany(f => f.Specializations)
                  .WithMany(s => s.Freelancers);

            builder.HasMany(f => f.Projects)
                  .WithOne(p => p.Freelancer)
                  .HasForeignKey(p => p.FreelancerId);

            builder.HasMany(f => f.FreelancerLanguagesPairs)
                  .WithOne(fl => fl.Freelancer)
                  .HasForeignKey(fl => fl.FreelancerId);

            builder.HasMany(f => f.Proposals)
                  .WithOne(pr => pr.Freelancer)
                  .HasForeignKey(pr => pr.FreelancerId);

            builder.HasMany(f => f.PaymentMethods)
                .WithOne(p => p.Freelancer)
                .HasForeignKey(p => p.FreelancerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsOne(f => f.Free, nav =>
            {
                nav.Property(e => e.HasTakenExam).HasColumnName("Free_HasTakenExam");
                nav.Property(e => e.Mark).HasColumnName("Free_Mark");
                nav.WithOwner();
            }).Navigation(e => e.Free).IsRequired(false);

            builder.OwnsOne(f => f.Pro1, nav =>
            {
                nav.Property(e => e.HasTakenExam).HasColumnName("Pro1_HasTakenExam");
                nav.Property(e => e.Mark).HasColumnName("Pro1_Mark");
                nav.WithOwner();
            }).Navigation(e => e.Pro1).IsRequired(false);

            builder.OwnsOne(f => f.Pro2, nav =>
            {
                nav.Property(e => e.HasTakenExam).HasColumnName("Pro2_HasTakenExam");
                nav.Property(e => e.Mark).HasColumnName("Pro2_Mark");
                nav.WithOwner();
            }).Navigation(e => e.Pro2).IsRequired(false);
        }
    }
}
